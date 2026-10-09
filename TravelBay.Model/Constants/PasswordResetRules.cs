namespace TravelBay.Model.Constants
{
    /// <summary>Limits of the emailed password reset code ("Zaboravljena lozinka").</summary>
    public static class PasswordResetRules
    {
        public const int CodeLength = 6;
        public const int CodeValidityMinutes = 15;
        public const int MaxFailedAttempts = 5;

        /// <summary>A new code for the same account is sent at most once per this many seconds.</summary>
        public const int ResendCooldownSeconds = 60;
    }
}
