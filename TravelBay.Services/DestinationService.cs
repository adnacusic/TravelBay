using System;
using System.Collections.Generic;
using System.Linq;
using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services;

public class DestinationService
    : BaseCRUDService<Destination, DestinationResponse, DestinationSearchObject, DestinationInsertRequest, DestinationUpdateRequest>,
        IDestinationService
{
    public DestinationService(
        TravelBayDbContext dbContext,
        MapsterMapper.IMapper mapper,
        IValidator<DestinationInsertRequest> insertValidator,
        IValidator<DestinationUpdateRequest> updateValidator)
        : base(dbContext, mapper, insertValidator, updateValidator)
    {
    }

    protected override Task<IQueryable<Destination>> IncludeRelatedEntitiesAsync(DestinationSearchObject? search, IQueryable<Destination> query = null!)
    {
        if (search?.IncludeCategory == true)
        {
            query = query.Include(d => d.Category);
        }
        if (search?.IncludeImages == true)
        {
            query = query.Include(d => d.Images);
        }
        return base.IncludeRelatedEntitiesAsync(search, query);
    }

    protected override IQueryable<Destination> ApplyFilters(IQueryable<Destination> query, DestinationSearchObject? search)
    {
        if (search != null)
        {
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                query = query.Where(d => d.Name.Contains(search.Name));
            }
            if (!string.IsNullOrWhiteSpace(search.Description))
            {
                query = query.Where(d => d.Description.Contains(search.Description));
            }
            if (search.CategoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == search.CategoryId.Value);
            }
            if (search.CityId.HasValue)
            {
                query = query.Where(d => d.CityId == search.CityId.Value);
            }
        }

        return query;
    }

    public override async Task<DestinationResponse> GetByIdAsync(int id)
    {
        var entity = await _dbContext.Destinations
            .AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.Images)
            .Include(d => d.Reviews.Where(r => r.Status == ReviewStatus.Approved))
            .FirstOrDefaultAsync(d => d.Id == id);

        if (entity == null)
        {
            throw new NotFoundException($"Destination with id {id} not found.");
        }

        var response = _mapper.Map<DestinationResponse>(entity);
        response.ReviewCount = entity.Reviews.Count;
        response.AverageRating = entity.Reviews.Count > 0 ? entity.Reviews.Average(r => r.Rating) : null;

        return response;
    }

    /// <summary>Business entities use soft delete (status flip), never a hard SQL DELETE.</summary>
    public override async Task DeleteAsync(int id)
    {
        var entity = await _dbContext.Destinations.FindAsync(id);
        if (entity == null)
        {
            throw new NotFoundException($"Destination with id {id} not found.");
        }

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }
}
