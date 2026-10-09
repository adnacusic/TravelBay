using TravelBay.Model.Requests;

namespace TravelBay.Services;

/// <summary>"Zaboravljena lozinka": a code is emailed (through the worker) and exchanged for a new password.</summary>
public interface IPasswordResetService
{
    /// <summary>Same outcome whether or not the email belongs to an account, so accounts cannot be probed.</summary>
    Task RequestCodeAsync(ForgotPasswordRequest request);

    Task ResetPasswordAsync(ResetPasswordRequest request);
}
