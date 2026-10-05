using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBay.Services
{
    public class AssetService : BaseCRUDService<Asset, AssetResponse, AssetSearch, AssetInsertRequest, AssetUpdateRequest>, IAssetService
    {
        public AssetService(TravelBayDbContext dbContext, IMapper mapper, IValidator<AssetInsertRequest> insertValidator, IValidator<AssetUpdateRequest> updateValidator)
           : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IQueryable<Asset> ApplyFilters(IQueryable<Asset> query, AssetSearch? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.FileName))
                {
                    query = query.Where(a => a.FileName.Contains(search.FileName));
                }

                if (!string.IsNullOrWhiteSpace(search.ContentType))
                {
                    query = query.Where(a => a.ContentType.Contains(search.ContentType));
                }
            }

            return query;
        }
    }
}
