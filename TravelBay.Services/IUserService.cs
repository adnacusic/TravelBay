using TravelBay.Model.Access;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services
{
    public interface IUserService : IBaseCRUDService<UserResponse, UserSearch, UserInsertRequest, UserUpdateRequest>
    {
        Task<UserSensitveResponse?> GetByUsernameAsync(string username);
        Task<UserResponse?> GetWithRoleByIdAsync(int id);

        /// <summary>Own-profile edit; never touches account status or role.</summary>
        Task<UserResponse> UpdateProfileAsync(int userId, UserProfileUpdateRequest request);

        /// <summary>Admin (de)activation; an admin can never deactivate their own account.</summary>
        Task<UserResponse> SetActiveAsync(int id, bool isActive, int currentUserId);

        Task<UserStatsResponse> GetStatsAsync();
        Task<UserActivityResponse> GetActivityAsync(int id);

        Task RecordLoginAsync(int userId);

        /// <summary>Replaces the profile picture; the previous Asset is removed.</summary>
        Task<UserResponse> SetProfileImageAsync(int userId, UserProfileImageRequest request);

        Task<UserResponse> RemoveProfileImageAsync(int userId);

        Task<(byte[] Content, string ContentType)> GetProfileImageAsync(int userId);

        /// <summary>Current-user self-service change; verifies the old password first.</summary>
        Task ChangePasswordAsync(int userId, UserPasswordChangeRequest request);

        /// <summary>Admin resetting another user's password; no old password check.</summary>
        Task ResetPasswordAsync(int userId, UserPasswordResetRequest request);
    }
}
