using TravelBay.Model.Constants;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class NewsController : BaseCRUDController<NewsResponse, NewsSearchObject, NewsInsertRequest, NewsUpdateRequest, INewsService>
{
    public NewsController(INewsService newsService) : base(newsService)
    {
    }

    [AllowAnonymous]
    public override Task<PageResult<NewsResponse>> GetAll([FromQuery] NewsSearchObject? search) => base.GetAll(search);

    [AllowAnonymous]
    public override Task<ActionResult<NewsResponse>> GetById(int id) => base.GetById(id);

    [AllowAnonymous]
    [HttpGet("{id}/Image")]
    public async Task<IActionResult> GetImage(int id)
    {
        var (content, contentType) = await _service.GetImageAsync(id);
        return File(content, contentType);
    }
}
