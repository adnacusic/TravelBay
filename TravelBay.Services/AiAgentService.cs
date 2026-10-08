using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelBay.Model.Constants;
using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using TravelBay.Services.Messaging;

namespace TravelBay.Services;

/// <summary>
/// The API never runs the agents itself: it records the run and hands it to the separate worker
/// container over RabbitMQ. The worker writes the progress back into the same AiAgentRun row.
/// </summary>
public class AiAgentService : IAiAgentService
{
    private static readonly AiAgentRunStatus[] ActiveStatuses = { AiAgentRunStatus.Queued, AiAgentRunStatus.Running };

    private readonly TravelBayDbContext _dbContext;
    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly IAiAgentQueuePublisher _publisher;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AiAgentService> _logger;

    public AiAgentService(
        TravelBayDbContext dbContext,
        IAuthenticatedUserAccessor userAccessor,
        IAiAgentQueuePublisher publisher,
        IAuditLogService auditLogService,
        ILogger<AiAgentService> logger)
    {
        _dbContext = dbContext;
        _userAccessor = userAccessor;
        _publisher = publisher;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    private static readonly Expression<Func<AiAgentRun, AiAgentRunResponse>> ToResponse = r => new AiAgentRunResponse
    {
        Id = r.Id,
        AgentType = r.AgentType,
        Status = r.Status,
        RequestedByDisplayName = r.RequestedByUser.FirstName + " " + r.RequestedByUser.LastName,
        RequestedAt = r.RequestedAt,
        StartedAt = r.StartedAt,
        FinishedAt = r.FinishedAt,
        TotalCount = r.TotalCount,
        ProcessedCount = r.ProcessedCount,
        SucceededCount = r.SucceededCount,
        FailedCount = r.FailedCount,
        Log = r.Log
    };

    private static string Label(AiAgentType agentType) => agentType switch
    {
        AiAgentType.Keywords => "AIAgentKeywords",
        AiAgentType.Images => "AIAgentSlike",
        _ => agentType.ToString()
    };

    private static string Command(AiAgentType agentType) => agentType switch
    {
        AiAgentType.Keywords => AiAgentMessages.GenerateKeywords,
        AiAgentType.Images => AiAgentMessages.FindImages,
        _ => throw new ClientException("Nepoznat AI agent.")
    };

    public async Task<AiAgentStatusResponse> GetStatusAsync()
    {
        return new AiAgentStatusResponse
        {
            DestinationsWithoutKeywords = await _dbContext.Destinations
                .CountAsync(d => d.Keywords == null || d.Keywords.Trim() == string.Empty),
            DestinationsWithoutImages = await _dbContext.Destinations.CountAsync(d => !d.Images.Any()),
            LastKeywordsRun = await LastRunAsync(AiAgentType.Keywords),
            LastImagesRun = await LastRunAsync(AiAgentType.Images)
        };
    }

    private async Task<AiAgentRunResponse?> LastRunAsync(AiAgentType agentType) =>
        await _dbContext.AiAgentRuns
            .AsNoTracking()
            .Where(r => r.AgentType == agentType)
            .OrderByDescending(r => r.Id)
            .Select(ToResponse)
            .FirstOrDefaultAsync();

    public async Task<AiAgentRunResponse> StartAsync(AiAgentType agentType)
    {
        var command = Command(agentType);
        var adminId = _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");

        var alreadyActive = await _dbContext.AiAgentRuns
            .AnyAsync(r => r.AgentType == agentType && ActiveStatuses.Contains(r.Status));
        if (alreadyActive)
        {
            throw new BusinessException($"{Label(agentType)} je već pokrenut. Sačekajte da završi prije novog pokretanja.");
        }

        var waiting = agentType == AiAgentType.Keywords
            ? await _dbContext.Destinations.CountAsync(d => d.Keywords == null || d.Keywords.Trim() == string.Empty)
            : await _dbContext.Destinations.CountAsync(d => !d.Images.Any());
        if (waiting == 0)
        {
            throw new BusinessException(agentType == AiAgentType.Keywords
                ? "Sve destinacije već imaju ključne riječi — AIAgentKeywords nema šta obraditi."
                : "Sve destinacije već imaju bar jednu sliku — AIAgentSlike nema šta obraditi.");
        }

        var run = new AiAgentRun
        {
            AgentType = agentType,
            Status = AiAgentRunStatus.Queued,
            RequestedByUserId = adminId,
            RequestedAt = DateTime.UtcNow,
            Log = $"[{DateTime.UtcNow:HH:mm:ss}] Poruka poslana workeru (čeka obradu: {waiting})."
        };
        _dbContext.AiAgentRuns.Add(run);
        await _dbContext.SaveChangesAsync();

        try
        {
            await _publisher.PublishAsync(command, run.Id);
        }
        catch (Exception ex) when (ex is RabbitMQ.Client.Exceptions.BrokerUnreachableException
                                       or RabbitMQ.Client.Exceptions.OperationInterruptedException
                                       or RabbitMQ.Client.Exceptions.AlreadyClosedException)
        {
            _logger.LogError(ex, "Publishing AI agent run {RunId} ({Command}) to RabbitMQ failed.", run.Id, command);
            run.Status = AiAgentRunStatus.Failed;
            run.FinishedAt = DateTime.UtcNow;
            run.Log += $"\n[{DateTime.UtcNow:HH:mm:ss}] RabbitMQ nije dostupan — poruka nije poslana.";
            await _dbContext.SaveChangesAsync();
            throw new BusinessException("Red poruka (RabbitMQ) trenutno nije dostupan, agent nije pokrenut. Pokušajte ponovo za nekoliko trenutaka.");
        }

        await _auditLogService.LogAsync(nameof(AiAgentRun), run.Id, $"Started:{agentType}", adminId, $"{Label(agentType)} queued, {waiting} destinations waiting.");
        _logger.LogInformation("AI agent run {RunId} ({Command}) published by user {UserId}.", run.Id, command, adminId);

        return await GetRunAsync(run.Id);
    }

    public async Task<PageResult<AiAgentRunResponse>> GetRunsAsync(AiAgentRunSearchObject? search)
    {
        search ??= new AiAgentRunSearchObject();

        var query = _dbContext.AiAgentRuns.AsNoTracking();
        if (search.AgentType.HasValue)
        {
            query = query.Where(r => r.AgentType == search.AgentType.Value);
        }

        int? totalCount = search.IncludeTotalCount == true ? await query.CountAsync() : null;
        var page = search.Page ?? 1;
        var pageSize = search.PageSize ?? PagingDefaults.DefaultPageSize;

        var items = await query
            .OrderByDescending(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse)
            .ToListAsync();

        return new PageResult<AiAgentRunResponse> { Items = items, TotalCount = totalCount };
    }

    public async Task<AiAgentRunResponse> GetRunAsync(int id)
    {
        return await _dbContext.AiAgentRuns
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(ToResponse)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException($"AiAgentRun with id {id} not found.");
    }
}
