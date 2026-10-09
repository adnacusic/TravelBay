using TravelBay.Model.Exceptions;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services
{
    /// <summary>
    /// Generic base service for CRUD operations (Create, Read, Update, Delete)
    /// </summary>
    public abstract class BaseCRUDService<TEntity, TResponse, TSearch, TInsertRequest, TUpdateRequest>
        : BaseReadService<TEntity, TResponse, TSearch>
        where TEntity : class
        where TSearch : BaseSearchObject, new()
    {
        private const string CreatedAtProperty = "CreatedAt";
        private const string UpdatedAtProperty = "UpdatedAt";

        /// <summary>SQL Server error number of a FK/CHECK constraint violation.</summary>
        private const int ForeignKeyViolationError = 547;

        protected readonly IValidator<TInsertRequest> _insertValidator;
        protected readonly IValidator<TUpdateRequest> _updateValidator;

        protected BaseCRUDService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<TInsertRequest> insertValidator, IValidator<TUpdateRequest> updateValidator) : base(mapper, dbContext)
        {
            _insertValidator = insertValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Maps an insert request to an entity. Override in derived classes for custom logic.
        /// </summary>
        protected virtual TEntity MapInsertRequestToEntity(TInsertRequest request)
        {
            return _mapper.Map<TEntity>(request ?? throw new ArgumentNullException(nameof(request)));
        }

        /// <summary>
        /// Maps an update request to an existing entity. Override in derived classes for custom logic.
        /// </summary>
        protected virtual void MapUpdateRequestToEntity(TUpdateRequest request, TEntity entity)
        {
            _mapper.Map(request, entity);
        }

        /// <summary>
        /// Inserts a new entity into the data source.
        /// </summary>
        public virtual async Task<TResponse> InsertAsync(TInsertRequest request)
        {
            await _insertValidator.ValidateAndThrowAsync(request);

            var entity = MapInsertRequestToEntity(request);

            // Entities with a CreatedAt column get it set here, so no service forgets it.
            var createdAtProperty = typeof(TEntity).GetProperty(CreatedAtProperty);
            if (createdAtProperty?.CanWrite == true)
            {
                createdAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<TResponse>(entity);
        }

        /// <summary>
        /// Updates an existing entity in the data source.
        /// </summary>
        public virtual async Task<TResponse> UpdateAsync(int id, TUpdateRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var entity = await _dbContext.Set<TEntity>().FindAsync(id)
                ?? throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found.");

            MapUpdateRequestToEntity(request, entity);

            var updatedAtProperty = typeof(TEntity).GetProperty(UpdatedAtProperty);
            if (updatedAtProperty?.CanWrite == true)
            {
                updatedAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<TResponse>(entity);
        }

        /// <summary>
        /// Deletes an entity from the data source by id.
        /// </summary>
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id)
                ?? throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found.");

            _dbContext.Set<TEntity>().Remove(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
                throw new ClientException(
                    $"{typeof(TEntity).Name} cannot be deleted because other records still reference it.");
            }
        }

        private static bool IsForeignKeyViolation(DbUpdateException ex) =>
            ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == ForeignKeyViolationError;
    }
}
