using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>Adds the city count per country with one grouped query for the whole page.</summary>
        public override async Task<PageResult<CountryResponse>> GetAllAsync(CountrySearchObject? search = null)
        {
            var result = await base.GetAllAsync(search);
            var ids = result.Items.Select(c => c.Id).ToList();

            var counts = await _dbContext.Cities
                .AsNoTracking()
                .Where(c => ids.Contains(c.CountryId))
                .GroupBy(c => c.CountryId)
                .Select(g => new { CountryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CountryId, x => x.Count);

            foreach (var item in result.Items)
            {
                item.CityCount = counts.GetValueOrDefault(item.Id);
            }

            return result;
        }

        public override async Task<CountryResponse> InsertAsync(CountryInsertRequest request)
        {
            await EnsureUniqueNameAsync(request.Name, exceptId: null);
            return await base.InsertAsync(request);
        }

        public override async Task<CountryResponse> UpdateAsync(int id, CountryUpdateRequest request)
        {
            await EnsureUniqueNameAsync(request.Name, exceptId: id);
            return await base.UpdateAsync(id, request);
        }

        public override async Task DeleteAsync(int id)
        {
            if (await _dbContext.Cities.AnyAsync(c => c.CountryId == id))
            {
                throw new ClientException("Država se ne može obrisati dok ima gradova. Prvo obrišite njene gradove.");
            }

            await base.DeleteAsync(id);
        }

        private async Task EnsureUniqueNameAsync(string name, int? exceptId)
        {
            var trimmed = name.Trim();
            if (await _dbContext.Countries.AnyAsync(c => c.Name == trimmed && c.Id != exceptId))
            {
                throw new ClientException($"Država \"{trimmed}\" već postoji.");
            }
        }
    }
}
