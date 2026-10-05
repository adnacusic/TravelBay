using TravelBay.Common.Services.CryptoService;
using TravelBay.Model.Access;
using TravelBay.Model.Constants;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBay.Services
{
    public class UserService : BaseCRUDService<User, UserResponse, UserSearch, UserInsertRequest, UserUpdateRequest>, IUserService
    {
        private readonly ICryptoService _cryptoService;
        public UserService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<UserInsertRequest> insertValidator, IValidator<UserUpdateRequest> updateValidator, ICryptoService cryptoService)
            : base(dbContext, mapper, insertValidator, updateValidator)
        {
            _cryptoService = cryptoService;
        }


        protected override IQueryable<User> ApplyFilters(IQueryable<User> query, UserSearch? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Email))
                {
                    query = query.Where(u => u.Email.Contains(search.Email));
                }

                if (!string.IsNullOrWhiteSpace(search.Username))
                {
                    query = query.Where(u => u.Username.Contains(search.Username));
                }

                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(u => u.FirstName.Contains(search.Name) || u.LastName.Contains(search.Name));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }

        protected override User MapInsertRequestToEntity(UserInsertRequest request)
        {
            var entity = base.MapInsertRequestToEntity(request);

            // Handle password hashing for User entity
            var salt = _cryptoService.GenerateSlat();
            entity.PasswordSalt = salt;
            entity.PasswordHash = _cryptoService.GenerateHash(request.Password, salt);

            return entity;
        }

        public override async Task<UserResponse> InsertAsync(UserInsertRequest request)
        {
            // let FluentValidation throw if the request isn't valid; the exception filter will
            // convert the resulting ValidationException into the standard error format.
            await _insertValidator.ValidateAndThrowAsync(request);

            // Check if email or username already exists
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new ClientException($"Email '{request.Email}' is already in use.");
            }

            if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username))
            {
                throw new ClientException($"Username '{request.Username}' is already in use.");
            }

            var entity = MapInsertRequestToEntity(request);
            entity.CreatedAt = DateTime.UtcNow;

            _dbContext.Users.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Every self-registered or admin-created user gets the default "User" role —
            // clients can never request a role directly (UserInsertRequest has no such field).
            var defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.User)
                ?? throw new InvalidOperationException($"Seed role '{RoleNames.User}' is missing.");

            _dbContext.UserRoles.Add(new UserRole
            {
                UserId = entity.Id,
                RoleId = defaultRole.Id,
                DateAssigned = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserResponse>(entity);
        }


        public override async Task<UserResponse> UpdateAsync(int id, UserUpdateRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var entity = await _dbContext.Users.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }

            // Check if email or username already exists
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email && u.Id != id))
            {
                throw new ClientException($"Email '{request.Email}' is already in use.");
            }

            if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username && u.Id != id))
            {
                throw new ClientException($"Username '{request.Username}' is already in use.");
            }

            MapUpdateRequestToEntity(request, entity);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserResponse>(entity);
        }

        /// <summary>Accounts are deactivated (soft), never hard-deleted — they're tied to reviews, trip plans, etc.</summary>
        public override async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Users.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }

            entity.IsActive = false;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserSensitveResponse?> GetByUsernameAsync(string username)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            UserSensitveResponse? response = null;

            if (user != null)
            {
                response = _mapper.Map<UserSensitveResponse>(user);
                response.Role = user.UserRoles.FirstOrDefault()?.Role.Name;
            }

            return response;
        }

        public async Task<UserResponse?> GetWithRoleByIdAsync(int id)
        {
            var user = await _dbContext.Users
               .AsNoTracking()
               .Include(u => u.UserRoles)
               .ThenInclude(ur => ur.Role)
               .FirstOrDefaultAsync(u => u.Id == id);

            UserResponse? response = null;

            if (user != null)
            {
                response = _mapper.Map<UserResponse>(user);
                response.Role = user.UserRoles.First().Role.Name;
            }

            return response;
        }

        public async Task ChangePasswordAsync(int userId, UserPasswordChangeRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException($"User with id {userId} not found.");
            }

            if (!_cryptoService.Verify(user.PasswordHash, user.PasswordSalt, request.Password))
            {
                throw new ClientException("Current password is incorrect.");
            }

            if (!string.Equals(request.NewPassword, request.ConfirmNewPassword, StringComparison.Ordinal))
            {
                throw new ClientException("Password confirmation does not match the new password.");
            }

            user.PasswordSalt = _cryptoService.GenerateSlat();
            user.PasswordHash = _cryptoService.GenerateHash(request.NewPassword, user.PasswordSalt);

            await _dbContext.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(int userId, UserPasswordResetRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException($"User with id {userId} not found.");
            }

            if (!string.Equals(request.NewPassword, request.ConfirmNewPassword, StringComparison.Ordinal))
            {
                throw new ClientException("Password confirmation does not match the new password.");
            }

            user.PasswordSalt = _cryptoService.GenerateSlat();
            user.PasswordHash = _cryptoService.GenerateHash(request.NewPassword, user.PasswordSalt);

            await _dbContext.SaveChangesAsync();
        }
    }
}
