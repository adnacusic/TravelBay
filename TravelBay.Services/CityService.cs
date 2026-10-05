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

        public override async Task<CityResponse> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Cities.Include(c => c.Country).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException($"City with id {id} not found.");

            return new CityResponse { Id = entity.Id, Name = entity.Name, CountryId = entity.CountryId, CountryName = entity.Country.Name };
        }
    }
}
