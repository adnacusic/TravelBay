using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using Humanizer;
using MapsterMapper;

namespace TravelBay.Services.ProductStateMachine
{   
    public class ActiveProductState : BaseProductState
    {
        public ActiveProductState(TravelBayDbContext dbContext, IMapper mapper, IServiceProvider serviceProvider) : base(dbContext, mapper, serviceProvider)
        {
            
        }

        override public async Task<ProductResponse> DeactivateAsync(int id)
        {
            var entity = await DbContext.Products.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }

            entity.ProductState = nameof(DraftProductState);    
            await DbContext.SaveChangesAsync();

            return Mapper.Map<ProductResponse>(entity);
        }

        public override List<string> GetAllowedActions()
        {
            return new List<string> { nameof(DeactivateAsync) };
        }
    }
}