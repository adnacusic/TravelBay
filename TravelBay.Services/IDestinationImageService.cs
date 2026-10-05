using TravelBay.Model.Requests;
using TravelBay.Model.Responses;

namespace TravelBay.Services;

public interface IDestinationImageService
{
    Task<DestinationImageResponse> InsertAsync(DestinationImageInsertRequest request);
    Task DeleteAsync(int id);

    /// <summary>Raw bytes of an admin-uploaded image (external images are served by their own host).</summary>
    Task<(byte[] Content, string ContentType)> GetContentAsync(int id);
}
