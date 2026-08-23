using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services.Database;
using Humanizer;
using MapsterMapper;

namespace TravelBay.Services.ProductStateMachine
{   
    public class InitialProductState : BaseProductState
    {
        public InitialProductState(TravelBayDbContext dbContext, IMapper mapper, IServiceProvider serviceProvider) : base(dbContext, mapper, serviceProvider)
        {
            
        }
        public override async Task<ProductResponse> InsertAsync(ProductInsertRequest request)
        {
            var entity = Mapper.Map<Product>(request);
            entity.ProductState = nameof(DraftProductState);
            DbContext.Products.Add(entity);
            await DbContext.SaveChangesAsync();
            var response = Mapper.Map<ProductResponse>(entity);
            return response;
        }

        public override List<string> GetAllowedActions()
        {
            return base.GetAllowedActions().Append(nameof(InsertAsync)).ToList();
        }
    }
}