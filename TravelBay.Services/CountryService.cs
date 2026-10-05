using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;

namespace TravelBay.Services
{
    public class CountryService : BaseCRUDService<Country, CountryResponse, CountrySearchObject, CountryInsertRequest, CountryUpdateRequest>, ICountryService
    {
        public CountryService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<CountryInsertRequest> insertValidator, IValidator<CountryUpdateRequest> updateValidator)
            : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IQueryable<Country> ApplyFilters(IQueryable<Country> query, CountrySearchObject? search)
        {
            if (!string.IsNullOrWhiteSpace(search?.Name))
            {
                query = query.Where(c => c.Name.Contains(search.Name));
            }

            return query;
        }
    }
}
