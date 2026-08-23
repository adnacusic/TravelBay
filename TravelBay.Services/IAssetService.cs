using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBay.Services
{
    public interface IAssetService : IBaseCRUDService<AssetResponse, AssetSearch, AssetInsertRequest, AssetUpdateRequest>
    {
    }
}
