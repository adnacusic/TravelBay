using System.Security.Cryptography;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelBay.Common.Services.CryptoService;
using TravelBay.Model.Constants;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Services.Database;
using TravelBay.Services.Messaging;

namespace TravelBay.Services;

public class PasswordResetService : IPasswordResetService
{
    private const string InvalidCodeMessage = "Kod nije ispravan ili je istekao. Zatražite novi kod.";

    private readonly TravelBayDbContext _dbContext;
    private readonly ICryptoService _cryptoService;
    private readonly IMessageQueuePublisher _publisher;
    private readonly SmtpSettings _smtpSettings;
    private readonly IValidator<ForgotPasswordRequest> _forgotValidator;
    private readonly IValidator<ResetPasswordRequest> _resetValidator;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        TravelBayDbContext dbContext,
        ICryptoService cryptoService,
        IMessageQueuePublisher publisher,
        SmtpSettings smtpSettings,
        IValidator<ForgotPasswordRequest> forgotValidator,
        IValidator<ResetPasswordRequest> resetValidator,
        ILogger<PasswordResetService> logger)
    {
        _dbContext = dbContext;
        _cryptoService = cryptoService;
        _publisher = publisher;
        _smtpSettings = smtpSettings;
        _forgotValidator = forgotValidator;
        _resetValidator = resetValidator;
        _logger = logger;
    }

    public async Task RequestCodeAsync(ForgotPasswordRequest request)
    {
        await _forgotValidator.ValidateAndThrowAsync(request);

        if (!_smtpSettings.IsConfigured)
        {
            throw new BusinessException("Slanje emaila nije podešeno (SMTP podaci nisu upisani u .env), pa kod za promjenu lozinke ne može biti poslan. Obratite se administratoru.");
        }

        var email = request.Email.Trim();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        if (user == null)
        {
            _logger.LogInformation("Password reset requested for an unknown or inactive email.");
            return;
        }

        var now = DateTime.UtcNow;
        var recentlySent = await _dbContext.PasswordResetCodes
            .AnyAsync(c => c.UserId == user.Id && c.CreatedAt > now.AddSeconds(-PasswordResetRules.ResendCooldownSeconds));
        if (recentlySent)
        {
            return;
        }

        // Only the newest code is valid.
        await _dbContext.PasswordResetCodes.Where(c => c.UserId == user.Id && c.UsedAt == null).ExecuteDeleteAsync();

        var code = RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, PasswordResetRules.CodeLength))
            .ToString($"D{PasswordResetRules.CodeLength}");
        var salt = _cryptoService.GenerateSlat();
        var entry = new PasswordResetCode
        {
            UserId = user.Id,
            CodeHash = _cryptoService.GenerateHash(code, salt),
            CodeSalt = salt,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(PasswordResetRules.CodeValidityMinutes)
        };
        _dbContext.PasswordResetCodes.Add(entry);
        await _dbContext.SaveChangesAsync();

        try
        {
            await _publisher.PublishAsync(EmailMessages.QueueName, new
            {
                command = EmailMessages.SendPasswordResetCode,
                email = user.Email,
                firstName = user.FirstName,
                code,
                validMinutes = PasswordResetRules.CodeValidityMinutes
            });
        }
        catch (Exception ex) when (ex is RabbitMQ.Client.Exceptions.BrokerUnreachableException
                                       or RabbitMQ.Client.Exceptions.OperationInterruptedException
                                       or RabbitMQ.Client.Exceptions.AlreadyClosedException)
        {
            _logger.LogError(ex, "Publishing the password reset email for user {UserId} failed.", user.Id);
            _dbContext.PasswordResetCodes.Remove(entry);
            await _dbContext.SaveChangesAsync();
            throw new BusinessException("Slanje emaila trenutno nije moguće. Pokušajte ponovo za nekoliko minuta.");
        }

        _logger.LogInformation("Password reset code for user {UserId} queued for email.", user.Id);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        await _resetValidator.ValidateAndThrowAsync(request);

        var email = request.Email.Trim();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive)
            ?? throw FieldError(nameof(request.Code), InvalidCodeMessage);

        var now = DateTime.UtcNow;
        var entry = await _dbContext.PasswordResetCodes
            .Where(c => c.UserId == user.Id && c.UsedAt == null)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
        if (entry == null || entry.ExpiresAt < now || entry.FailedAttempts >= PasswordResetRules.MaxFailedAttempts)
        {
            throw FieldError(nameof(request.Code), InvalidCodeMessage);
        }

        if (!_cryptoService.Verify(entry.CodeHash, entry.CodeSalt, request.Code))
        {
            entry.FailedAttempts++;
            await _dbContext.SaveChangesAsync();
            var left = PasswordResetRules.MaxFailedAttempts - entry.FailedAttempts;
            throw FieldError(nameof(request.Code), left > 0
                ? $"Kod nije ispravan. Preostalo pokušaja: {left}."
                : InvalidCodeMessage);
        }

        // New password, code used up and every session signed out — all or nothing.
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        user.PasswordSalt = _cryptoService.GenerateSlat();
        user.PasswordHash = _cryptoService.GenerateHash(request.NewPassword, user.PasswordSalt);
        entry.UsedAt = now;
        await _dbContext.SaveChangesAsync();
        await _dbContext.RefreshTokens.Where(rt => rt.UserId == user.Id).ExecuteDeleteAsync();
        await transaction.CommitAsync();

        _logger.LogInformation("Password of user {UserId} reset with an emailed code.", user.Id);
    }

    private static ValidationException FieldError(string propertyName, string message) =>
        new(new[] { new ValidationFailure(propertyName, message) });
}
