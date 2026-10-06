using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services
{
    public class CityService : BaseCRUDService<City, CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest>, ICityService
    {
        public CityService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<CityInsertRequest> insertValidator, IValidator<CityUpdateRequest> updateValidator)
            : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override Task<IQueryable<City>> IncludeRelatedEntitiesAsync(CitySearchObject? search, IQueryable<City> query)
        {
            return base.IncludeRelatedEntitiesAsync(search, query.Include(c => c.Country));
        }

        protected override IQueryable<City> ApplyFilters(IQueryable<City> query, CitySearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(c => c.Name.Contains(search.Name));
                }

                if (search.CountryId.HasValue)
                {
                    query = query.Where(c => c.CountryId == search.CountryId.Value);
                }
            }

            return query;
        }

        /// <summary>Adds the destination count per city with one grouped query for the whole page.</summary>
        public override async Task<PageResult<CityResponse>> GetAllAsync(CitySearchObject? search = null)
        {
            var result = await base.GetAllAsync(search);
            var ids = result.Items.Select(c => c.Id).ToList();

            var counts = await _dbContext.Destinations
                .AsNoTracking()
                .Where(d => ids.Contains(d.CityId))
                .GroupBy(d => d.CityId)
                .Select(g => new { CityId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CityId, x => x.Count);

            foreach (var item in result.Items)
            {
                item.DestinationCount = counts.GetValueOrDefault(item.Id);
            }

            return result;
        }

        public override async Task<CityResponse> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Cities.Include(c => c.Country).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException($"City with id {id} not found.");

            return new CityResponse { Id = entity.Id, Name = entity.Name, CountryId = entity.CountryId, CountryName = entity.Country.Name };
        }

        public override async Task<CityResponse> InsertAsync(CityInsertRequest request)
        {
            await EnsureUniqueNameAsync(request.Name, request.CountryId, exceptId: null);
            var created = await base.InsertAsync(request);
            return await GetByIdAsync(created.Id);
        }

        public override async Task<CityResponse> UpdateAsync(int id, CityUpdateRequest request)
        {
            await EnsureUniqueNameAsync(request.Name, request.CountryId, exceptId: id);
            await base.UpdateAsync(id, request);
            return await GetByIdAsync(id);
        }

        /// <summary>Soft-deleted destinations still reference the city, so they block deletion too.</summary>
        public override async Task DeleteAsync(int id)
        {
            if (await _dbContext.Destinations.IgnoreQueryFilters().AnyAsync(d => d.CityId == id))
            {
                throw new ClientException("Grad se ne može obrisati jer ga koriste destinacije.");
            }

            await base.DeleteAsync(id);
        }

        private async Task EnsureUniqueNameAsync(string name, int countryId, int? exceptId)
        {
            var trimmed = name.Trim();
            if (await _dbContext.Cities.AnyAsync(c => c.Name == trimmed && c.CountryId == countryId && c.Id != exceptId))
            {
                throw new ClientException($"Grad \"{trimmed}\" već postoji u odabranoj državi.");
            }
        }
    }
}
