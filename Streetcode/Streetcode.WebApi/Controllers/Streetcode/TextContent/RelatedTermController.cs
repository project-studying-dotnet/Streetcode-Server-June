using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.DeleteById;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAll;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent
{
    public class RelatedTermController : BaseApiController
    {
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Create([FromBody] RelatedTermCreateDTO relatedTerm)
        {
            return HandleResult(await Mediator.Send(new CreateRelatedTermCommand(relatedTerm)));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllRelatedTermsQuery()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetRelatedTermByIdQuery(id)));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAllByTermId([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetAllRelatedTermsByTermIdQuery(id)));
        }

        [HttpPut]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Update([FromBody] RelatedTermDTO relatedTerm)
        {
            return HandleResult(await Mediator.Send(new UpdateRelatedTermCommand(relatedTerm)));
        }

        [HttpDelete("{word}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Delete([FromRoute] string word)
        {
            return HandleResult(await Mediator.Send(new DeleteRelatedTermCommand(word)));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteRelatedTermByIdCommand(id)));
        }
    }
}
