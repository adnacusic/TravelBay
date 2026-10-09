using TravelBay.Model.Exceptions;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Reflection;
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
                query = query.OrderBy(SafeSortExpression(search.SortBy));
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

        /// <summary>Scalar columns of the entity (no navigations): the only names SortBy may use.</summary>
        private static readonly Dictionary<string, string> SortableColumns = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsScalar(p.PropertyType))
            .ToDictionary(p => p.Name, p => p.Name, StringComparer.OrdinalIgnoreCase);

        private static readonly string[] SortDirections = { "asc", "desc" };

        private static bool IsScalar(Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type) ?? type;
            return underlying.IsPrimitive || underlying.IsEnum || underlying == typeof(string)
                || underlying == typeof(decimal) || underlying == typeof(DateTime);
        }

        /// <summary>
        /// Rebuilds the client's SortBy from whitelisted parts only, so no expression text from the
        /// request ever reaches Dynamic LINQ: "lastName, FirstName desc" -> "LastName, FirstName desc".
        /// </summary>
        private static string SafeSortExpression(string sortBy)
        {
            var parts = new List<string>();
            foreach (var item in sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var tokens = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var direction = tokens.Length == 2 ? tokens[1].ToLowerInvariant() : null;
                if (tokens.Length > 2 || !SortableColumns.TryGetValue(tokens[0], out var column)
                    || (direction != null && !SortDirections.Contains(direction)))
                {
                    throw new ClientException($"Sortiranje po '{item}' nije podržano.");
                }
                parts.Add(direction == null ? column : $"{column} {direction}");
            }

            if (parts.Count == 0)
            {
                throw new ClientException("Sortiranje nije ispravno zadano.");
            }
            return string.Join(", ", parts);
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
