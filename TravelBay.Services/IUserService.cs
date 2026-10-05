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

        /// <summary>Current-user self-service change; verifies the old password first.</summary>
        Task ChangePasswordAsync(int userId, UserPasswordChangeRequest request);

        /// <summary>Admin resetting another user's password; no old password check.</summary>
        Task ResetPasswordAsync(int userId, UserPasswordResetRequest request);
    }
}
