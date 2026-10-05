using TravelBay.Model.Exceptions;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services
{
    public abstract class BaseReadService<TEntity, TResponse, TSearch> : IBaseReadService<TResponse, TSearch>
        where TEntity : class
        where TSearch : BaseSearchObject, new()
    {
        protected readonly MapsterMapper.IMapper _mapper;
        protected readonly TravelBayDbContext _dbContext;

        protected BaseReadService(MapsterMapper.IMapper mapper, TravelBayDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }


        /// <summary>
        /// Applies search filters to the query. Override in derived classes to implement specific
        /// filtering logic. The returned query must stay an <see cref="IQueryable{T}"/> (built from
        /// Where/etc.) so EF Core translates it into SQL instead of filtering in memory.
        /// </summary>
        protected abstract IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, TSearch? search);

        public virtual async Task<PageResult<TResponse>> GetAllAsync(TSearch? search = null)
        {
            search ??= new TSearch();

            IQueryable<TEntity> query = this._dbContext.Set<TEntity>();

            query = await IncludeRelatedEntitiesAsync(search, query);
            query = ApplyFilters(query, search);

            int? totalCount = null;

            if (search.IncludeTotalCount ?? false)
            {
                totalCount = await query.CountAsync();
            }

            if (!string.IsNullOrWhiteSpace(search.SortBy))
            {
                //TODO: parametrize sortBy to prevent SQL injection
                query = query.OrderBy(search.SortBy);
            }
            else
            {
                // Skip/Take without an ordering gives no stable page order; every entity has an Id key.
                query = query.OrderBy("Id");
            }

            if (search.Page.HasValue)
            {
                query = query.Skip((search.Page.Value - 1) * (search.PageSize ?? 10));
            }

            if (search.PageSize.HasValue)
            {
                query = query.Take(search.PageSize.Value);
            }

            var list = await query.Select(item => _mapper.Map<TResponse>(item)).ToListAsync();

            return new PageResult<TResponse>
            {
                Items = list,
                TotalCount = totalCount
            };
        }

        protected virtual Task<IQueryable<TEntity>> IncludeRelatedEntitiesAsync(TSearch? search, IQueryable<TEntity> query)
        {
            // Override in derived classes to include related entities if necessary
            return Task.FromResult(query);
        }


        public virtual async Task<TResponse> GetByIdAsync(int id)
        {
            var entity = await this._dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found.");
            }

            return _mapper.Map<TResponse>(entity);
        }
    }
}
