using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services
{
    public class CategoryService : BaseCRUDService<Category, CategoryResponse, CategorySearchObject, CategoriesInsertRequest, CategoriesUpdateRequest>, ICategoryService
    {
        public CategoryService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<CategoriesInsertRequest> insertValidator, IValidator<CategoriesUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IQueryable<Category> ApplyFilters(IQueryable<Category> query, CategorySearchObject? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(c => c.Name.Contains(search.Name));
                }
            }

            return query;
        }

        /// <summary>Adds the destination count per category with one grouped query for the whole page.</summary>
        public override async Task<PageResult<CategoryResponse>> GetAllAsync(CategorySearchObject? search = null)
        {
            var result = await base.GetAllAsync(search);
            var ids = result.Items.Select(c => c.Id).ToList();

            var counts = await _dbContext.Destinations
                .AsNoTracking()
                .Where(d => ids.Contains(d.CategoryId))
                .GroupBy(d => d.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

            foreach (var item in result.Items)
            {
                item.DestinationCount = counts.GetValueOrDefault(item.Id);
            }

            return result;
        }

        public override async Task<CategoryResponse> InsertAsync(CategoriesInsertRequest request)
        {
            await EnsureUniqueNameAsync(request.Name, exceptId: null);
            return await base.InsertAsync(request);
        }

        public override async Task<CategoryResponse> UpdateAsync(int id, CategoriesUpdateRequest request)
        {
            await EnsureUniqueNameAsync(request.Name, exceptId: id);
            return await base.UpdateAsync(id, request);
        }

        /// <summary>A category in use (also by soft-deleted destinations or user preferences) can only be deactivated.</summary>
        public override async Task DeleteAsync(int id)
        {
            var usedByDestinations = await _dbContext.Destinations.IgnoreQueryFilters().AnyAsync(d => d.CategoryId == id);
            var usedByPreferences = await _dbContext.UserPreferences.AnyAsync(p => p.CategoryId == id);
            if (usedByDestinations || usedByPreferences)
            {
                throw new ClientException(
                    "Kategorija se ne može obrisati jer je koriste destinacije ili korisničke preferencije. Umjesto brisanja je deaktivirajte.");
            }

            await base.DeleteAsync(id);
        }

        private async Task EnsureUniqueNameAsync(string name, int? exceptId)
        {
            var trimmed = name.Trim();
            if (await _dbContext.Categories.AnyAsync(c => c.Name == trimmed && c.Id != exceptId))
            {
                throw new ClientException($"Kategorija \"{trimmed}\" već postoji.");
            }
        }
    }
}
