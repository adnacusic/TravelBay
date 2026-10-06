using TravelBay.Model.Exceptions;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace TravelBay.Services
{
    public class NewsService : BaseCRUDService<News, NewsResponse, NewsSearchObject, NewsInsertRequest, NewsUpdateRequest>, INewsService
    {
        public NewsService(TravelBayDbContext dbContext, MapsterMapper.IMapper mapper, IValidator<NewsInsertRequest> insertValidator, IValidator<NewsUpdateRequest> updateValidator)
            : base(dbContext, mapper, insertValidator, updateValidator)
        {
        }

        /// <summary>API-relative path an uploaded image is served from (clients prefix it with their base URL).</summary>
        private static string ImagePath(int newsId) => $"News/{newsId}/Image";

        protected override IQueryable<News> ApplyFilters(IQueryable<News> query, NewsSearchObject? search)
        {
            if (!string.IsNullOrWhiteSpace(search?.Title))
            {
                query = query.Where(n => n.Title.Contains(search.Title));
            }

            return query;
        }

        /// <summary>Newest publication first unless the caller asks for another order.</summary>
        public override Task<PageResult<NewsResponse>> GetAllAsync(NewsSearchObject? search = null)
        {
            search ??= new NewsSearchObject();
            if (string.IsNullOrWhiteSpace(search.SortBy))
            {
                search.SortBy = "PublishedAt desc, Id desc";
            }

            return base.GetAllAsync(search);
        }

        public override async Task<NewsResponse> InsertAsync(NewsInsertRequest request)
        {
            await _insertValidator.ValidateAndThrowAsync(request);

            var news = new News
            {
                Title = request.Title.Trim(),
                Content = request.Content.Trim(),
                PublishedAt = request.PublishedAt.ToUniversalTime(),
                CreatedAt = DateTime.UtcNow
            };

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            _dbContext.News.Add(news);
            await SetImageAsync(news, request);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return _mapper.Map<NewsResponse>(news);
        }

        public override async Task<NewsResponse> UpdateAsync(int id, NewsUpdateRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var news = await _dbContext.News.Include(n => n.ImageAsset).FirstOrDefaultAsync(n => n.Id == id)
                ?? throw new NotFoundException($"News with id {id} not found.");

            news.Title = request.Title.Trim();
            news.Content = request.Content.Trim();
            news.PublishedAt = request.PublishedAt.ToUniversalTime();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            var hasNewImage = !string.IsNullOrWhiteSpace(request.ImageUrl) || !string.IsNullOrWhiteSpace(request.Base64Content);
            if (hasNewImage)
            {
                var previousAsset = news.ImageAsset;
                await SetImageAsync(news, request);
                if (previousAsset != null)
                {
                    _dbContext.Assets.Remove(previousAsset);
                }
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return _mapper.Map<NewsResponse>(news);
        }

        public override async Task DeleteAsync(int id)
        {
            var news = await _dbContext.News.Include(n => n.ImageAsset).FirstOrDefaultAsync(n => n.Id == id)
                ?? throw new NotFoundException($"News with id {id} not found.");

            _dbContext.News.Remove(news);
            if (news.ImageAsset != null)
            {
                _dbContext.Assets.Remove(news.ImageAsset);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<(byte[] Content, string ContentType)> GetImageAsync(int id)
        {
            var asset = await _dbContext.News
                .AsNoTracking()
                .Where(n => n.Id == id && n.ImageAssetId != null)
                .Select(n => new { n.ImageAsset!.Base64Content, n.ImageAsset.ContentType })
                .FirstOrDefaultAsync();

            if (asset == null)
            {
                throw new NotFoundException($"Uploaded image for news with id {id} not found.");
            }

            return (Convert.FromBase64String(asset.Base64Content), asset.ContentType);
        }

        /// <summary>Points the news at an external URL, or stores the uploaded file and points at its API path.</summary>
        private async Task SetImageAsync(News news, NewsInsertRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Base64Content))
            {
                news.ImageAsset = null;
                news.ImageAssetId = null;
                news.ImageUrl = request.ImageUrl!.Trim();
                return;
            }

            news.ImageAsset = new Asset
            {
                FileName = request.FileName!,
                ContentType = request.ContentType!,
                Base64Content = request.Base64Content,
                CreatedAt = DateTime.UtcNow
            };

            // The image path contains the news id, which only exists after the first save.
            if (news.Id == 0)
            {
                news.ImageUrl = string.Empty;
                await _dbContext.SaveChangesAsync();
            }

            news.ImageUrl = ImagePath(news.Id);
        }
    }
}
