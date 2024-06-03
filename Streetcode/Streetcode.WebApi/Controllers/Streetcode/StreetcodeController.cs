using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetById;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllShort;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetCount;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetShortById;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllStreetcodesMainPage;

namespace Streetcode.WebApi.Controllers.Streetcode;

public class StreetcodeController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStreetcodesRequestDTO request)
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesQuery(request)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShort()
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesShortQuery()));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMainPage()
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesMainPageQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShortById(int id)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeShortByIdQuery(id)));
    }

    [HttpGet]
    public async Task<IActionResult> GetByFilter([FromQuery] StreetcodeFilterRequestDTO request)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeByFilterQuery(request)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCatalog([FromQuery] int page, [FromQuery] int count)
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesCatalogQuery(page, count)));
    }

    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        return HandleResult(await Mediator.Send(new GetStreetcodesCountQuery()));
    }

    [HttpGet("{url}")]
    public async Task<IActionResult> GetByTransliterationUrl([FromRoute] string url)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeByTransliterationUrlQuery(url)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeByIdQuery(id)));
    }
}
