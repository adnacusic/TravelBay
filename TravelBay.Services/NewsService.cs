using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;

namespace TravelBay.Services
{
    public class NewsService : BaseCRUDService<News, NewsResponse, NewsSearchObject, NewsInsertRequest, NewsUpdateRequest>, INewsService
    {
        public NewsService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<NewsInsertRequest> insertValidator, IValidator<NewsUpdateRequest> updateValidator)
            : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        protected override IQueryable<News> ApplyFilters(IQueryable<News> query, NewsSearchObject? search)
        {
            if (!string.IsNullOrWhiteSpace(search?.Title))
            {
                query = query.Where(n => n.Title.Contains(search.Title));
            }

            return query;
        }
    }
}
