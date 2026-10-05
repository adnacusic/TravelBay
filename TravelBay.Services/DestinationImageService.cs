using TravelBay.Model.Constants;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class DestinationImageService : IDestinationImageService
{
    private readonly TravelBayDbContext _dbContext;
    private readonly MapsterMapper.IMapper _mapper;
    private readonly IValidator<DestinationImageInsertRequest> _insertValidator;

    public DestinationImageService(
        TravelBayDbContext dbContext,
        MapsterMapper.IMapper mapper,
        IValidator<DestinationImageInsertRequest> insertValidator)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _insertValidator = insertValidator;
    }

    /// <summary>API-relative path an uploaded image is served from (clients prefix it with their base URL).</summary>
    private static string ContentPath(int imageId) => $"DestinationImages/{imageId}/Content";

    public async Task<DestinationImageResponse> InsertAsync(DestinationImageInsertRequest request)
    {
        await _insertValidator.ValidateAndThrowAsync(request);

        var destinationExists = await _dbContext.Destinations.AnyAsync(d => d.Id == request.DestinationId);
        if (!destinationExists)
        {
            throw new NotFoundException($"Destination with id {request.DestinationId} not found.");
        }

        var maxOrderIndex = await _dbContext.DestinationImages
            .Where(i => i.DestinationId == request.DestinationId)
            .MaxAsync(i => (int?)i.OrderIndex);

        var image = new DestinationImage
        {
            DestinationId = request.DestinationId,
            OrderIndex = (maxOrderIndex ?? -1) + 1,
            IsAiGenerated = false,
            CreatedAt = DateTime.UtcNow
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        if (!string.IsNullOrWhiteSpace(request.Base64Content))
        {
            image.Source = ImageSources.AdminUpload;
            image.Asset = new Asset
            {
                FileName = request.FileName!,
                ContentType = request.ContentType!,
                Base64Content = request.Base64Content,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.DestinationImages.Add(image);
            await _dbContext.SaveChangesAsync();

            // The content path contains the image's own id, which only exists after the first save.
            image.ImageUrl = ContentPath(image.Id);
        }
        else
        {
            image.Source = ImageSources.AdminUrl;
            image.ImageUrl = request.ImageUrl!.Trim();
            _dbContext.DestinationImages.Add(image);
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return _mapper.Map<DestinationImageResponse>(image);
    }

    /// <summary>Images are child records of a destination (not a business process), so they are removed outright.</summary>
    public async Task DeleteAsync(int id)
    {
        var image = await _dbContext.DestinationImages
            .Include(i => i.Asset)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (image == null)
        {
            throw new NotFoundException($"Destination image with id {id} not found.");
        }

        _dbContext.DestinationImages.Remove(image);
        if (image.Asset != null)
        {
            _dbContext.Assets.Remove(image.Asset);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<(byte[] Content, string ContentType)> GetContentAsync(int id)
    {
        var asset = await _dbContext.DestinationImages
            .AsNoTracking()
            .Where(i => i.Id == id && i.AssetId != null)
            .Select(i => new { i.Asset!.Base64Content, i.Asset.ContentType })
            .FirstOrDefaultAsync();

        if (asset == null)
        {
            throw new NotFoundException($"Uploaded destination image with id {id} not found.");
        }

        return (Convert.FromBase64String(asset.Base64Content), asset.ContentType);
    }
}
