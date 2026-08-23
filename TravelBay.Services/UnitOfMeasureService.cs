using System;
using System.Collections.Generic;
using System.Linq;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;

namespace TravelBay.Services
{
    public class UnitOfMeasureService : BaseCRUDService<UnitOfMeasure, UnitOfMeasureResponse, UnitOfMeasureSearch, UnitOfMeasureInsertRequest, UnitOfMeasureUpdateRequest>, IUnitOfMeasureService
    {
        public UnitOfMeasureService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<UnitOfMeasureInsertRequest> insertValidator, IValidator<UnitOfMeasureUpdateRequest> updateValidator) : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IEnumerable<UnitOfMeasure> ApplyFilters(IEnumerable<UnitOfMeasure> query, UnitOfMeasureSearch? search)
        {
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(u => u.Name.Contains(search.Name, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(search.Abbreviation))
                {
                    query = query.Where(u => u.Abbreviation.Contains(search.Abbreviation, StringComparison.OrdinalIgnoreCase));
                }

                if (search.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == search.IsActive.Value);
                }
            }

            return query;
        }
    }
}
