using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

public interface ICollectionService : IBaseCRUDService<CollectionResponse, CollectionSearchObject, CollectionInsertRequest, CollectionUpdateRequest>
{
    Task<List<CollectionItemResponse>> GetItemsAsync(int collectionId);
    Task<CollectionItemResponse> AddItemAsync(int collectionId, CollectionItemInsertRequest request);
    Task RemoveItemAsync(int collectionId, int itemId);
}
