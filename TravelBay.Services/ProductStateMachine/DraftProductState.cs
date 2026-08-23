using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using Humanizer;
using MapsterMapper;

namespace TravelBay.Services.ProductStateMachine
{   
    public class DraftProductState : BaseProductState
    {
        public DraftProductState(TravelBayDbContext dbContext, IMapper mapper, IServiceProvider serviceProvider) : base(dbContext, mapper, serviceProvider)
        {
            
        }
        public override async Task<ProductResponse> UpdateAsync(int id, ProductUpdateRequest request)
        {
            var entity = await DbContext.Products.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }

            Mapper.Map(request, entity);
            //todo: add validations ...
            DbContext.SaveChanges();

            var response = Mapper.Map<ProductResponse>(entity);
            return response;
        }

        public override async Task<ProductResponse> ActivateAsync(int id)
        {
            var entity = await DbContext.Products.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }

            entity.ProductState = nameof(ActiveProductState);

            //todo: add validations ...
            DbContext.SaveChanges();

            var response = Mapper.Map<ProductResponse>(entity);
            return response;
        }

        public override async Task<ProductResponse> DeleteAsync(int id)
        {
            var entity = await DbContext.Products.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }

            DbContext.Products.Remove(entity);
            DbContext.SaveChanges();

            return Mapper.Map<ProductResponse>(entity);
        }

        public override List<string> GetAllowedActions()
        {
            return new List<string> { nameof(UpdateAsync), nameof(ActivateAsync), nameof(DeleteAsync) };
        }
    }
}