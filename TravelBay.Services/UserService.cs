using TravelBay.Common.Services.CryptoService;
using TravelBay.Model.Constants;
using TravelBay.Model.Enums;
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
        private const int NewUsersPeriodDays = 30;

        private readonly ICryptoService _cryptoService;
        private readonly IValidator<UserProfileUpdateRequest> _profileValidator;
        private readonly IValidator<UserPasswordChangeRequest> _passwordChangeValidator;
        private readonly IValidator<UserPasswordResetRequest> _passwordResetValidator;

        public UserService(
            TravelBayDbContext dbContext,
            MapsterMapper.IMapper mapper,
            IValidator<UserInsertRequest> insertValidator,
            IValidator<UserUpdateRequest> updateValidator,
            IValidator<UserProfileUpdateRequest> profileValidator,
            IValidator<UserPasswordChangeRequest> passwordChangeValidator,
            IValidator<UserPasswordResetRequest> passwordResetValidator,
            ICryptoService cryptoService)
            : base(dbContext, mapper, insertValidator, updateValidator)
        {
            _cryptoService = cryptoService;
            _profileValidator = profileValidator;
            _passwordChangeValidator = passwordChangeValidator;
            _passwordResetValidator = passwordResetValidator;
        }

        /// <summary>Roles are part of every user response (mapped to UserResponse.Role).</summary>
        protected override Task<IQueryable<User>> IncludeRelatedEntitiesAsync(UserSearch? search, IQueryable<User> query)
        {
            return base.IncludeRelatedEntitiesAsync(search, query.Include(u => u.UserRoles).ThenInclude(ur => ur.Role));
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

                if (!string.IsNullOrWhiteSpace(search.SearchText))
                {
                    var text = search.SearchText.Trim();
                    query = query.Where(u => u.FirstName.Contains(text)
                        || u.LastName.Contains(text)
                        || u.Username.Contains(text)
                        || u.Email.Contains(text));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }

        public override async Task<UserResponse> GetByIdAsync(int id)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }

            return _mapper.Map<UserResponse>(user);
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

            await EnsureUniqueAsync(request.Email, request.Username, exceptUserId: null);

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

            await EnsureUniqueAsync(request.Email, request.Username, exceptUserId: id);

            MapUpdateRequestToEntity(request, entity);

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<UserResponse> UpdateProfileAsync(int userId, UserProfileUpdateRequest request)
        {
            await _profileValidator.ValidateAndThrowAsync(request);

            var entity = await _dbContext.Users.FindAsync(userId)
                ?? throw new NotFoundException($"User with id {userId} not found.");

            var email = request.Email.Trim();
            var username = request.Username.Trim();
            await EnsureUniqueAsync(email, username, exceptUserId: userId);

            entity.FirstName = request.FirstName.Trim();
            entity.LastName = request.LastName.Trim();
            entity.Email = email;
            entity.Username = username;
            entity.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(userId);
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

        public async Task<UserResponse> SetActiveAsync(int id, bool isActive, int currentUserId)
        {
            if (!isActive && id == currentUserId)
            {
                throw new ClientException("You cannot deactivate your own account.");
            }

            var entity = await _dbContext.Users.FindAsync(id)
                ?? throw new NotFoundException($"User with id {id} not found.");

            entity.IsActive = isActive;
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<UserStatsResponse> GetStatsAsync()
        {
            var newSince = DateTime.UtcNow.AddDays(-NewUsersPeriodDays);

            var counts = await _dbContext.Users
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Active = g.Count(u => u.IsActive),
                    New = g.Count(u => u.CreatedAt >= newSince)
                })
                .FirstOrDefaultAsync();

            var administrators = await _dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.Role.Name == RoleNames.Admin)
                .Select(ur => ur.UserId)
                .Distinct()
                .CountAsync();

            var total = counts?.Total ?? 0;
            var active = counts?.Active ?? 0;

            return new UserStatsResponse
            {
                TotalUsers = total,
                ActiveUsers = active,
                InactiveUsers = total - active,
                Administrators = administrators,
                NewUsers = counts?.New ?? 0,
                NewUsersPeriodDays = NewUsersPeriodDays
            };
        }

        public async Task<UserActivityResponse> GetActivityAsync(int id)
        {
            var exists = await _dbContext.Users.AnyAsync(u => u.Id == id);
            if (!exists)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }

            var reviews = await _dbContext.Reviews
                .AsNoTracking()
                .Where(r => r.UserId == id)
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int ReviewsWith(ReviewStatus status) => reviews.Where(r => r.Status == status).Sum(r => r.Count);

            return new UserActivityResponse
            {
                ReviewCount = reviews.Sum(r => r.Count),
                PendingReviewCount = ReviewsWith(ReviewStatus.Pending),
                ApprovedReviewCount = ReviewsWith(ReviewStatus.Approved),
                RejectedReviewCount = ReviewsWith(ReviewStatus.Rejected),
                TripPlanCount = await _dbContext.TripPlans.CountAsync(t => t.UserId == id),
                CollectionCount = await _dbContext.Collections.CountAsync(c => c.UserId == id),
                SavedDestinationCount = await _dbContext.SavedDestinations.CountAsync(s => s.UserId == id),
                ViewCount = await _dbContext.ViewHistories.CountAsync(v => v.UserId == id)
            };
        }

        public async Task RecordLoginAsync(int userId)
        {
            var entity = await _dbContext.Users.FindAsync(userId);
            if (entity != null)
            {
                entity.LastLoginAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
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
            await _passwordChangeValidator.ValidateAndThrowAsync(request);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException($"User with id {userId} not found.");
            }

            if (!_cryptoService.Verify(user.PasswordHash, user.PasswordSalt, request.Password))
            {
                throw new ClientException("Current password is incorrect.");
            }

            user.PasswordSalt = _cryptoService.GenerateSlat();
            user.PasswordHash = _cryptoService.GenerateHash(request.NewPassword, user.PasswordSalt);

            await _dbContext.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(int userId, UserPasswordResetRequest request)
        {
            await _passwordResetValidator.ValidateAndThrowAsync(request);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException($"User with id {userId} not found.");
            }

            user.PasswordSalt = _cryptoService.GenerateSlat();
            user.PasswordHash = _cryptoService.GenerateHash(request.NewPassword, user.PasswordSalt);

            await _dbContext.SaveChangesAsync();
        }

        private async Task EnsureUniqueAsync(string? email, string? username, int? exceptUserId)
        {
            if (email != null && await _dbContext.Users.AnyAsync(u => u.Email == email && u.Id != exceptUserId))
            {
                throw new ClientException($"Email '{email}' is already in use.");
            }

            if (username != null && await _dbContext.Users.AnyAsync(u => u.Username == username && u.Id != exceptUserId))
            {
                throw new ClientException($"Username '{username}' is already in use.");
            }
        }
    }
}
