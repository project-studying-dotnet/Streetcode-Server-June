using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Text.GetParsed
{
    public class GetParsedTextAdminPreviewHandler : IRequestHandler<GetParsedTextForAdminPreviewCommand, Result<string>>
    {
        private readonly ITextService _textService;

        public GetParsedTextAdminPreviewHandler(ITextService textService)
        {
            _textService = textService;
        }

        public async Task<Result<string>> Handle(GetParsedTextForAdminPreviewCommand request, CancellationToken cancellationToken)
        {
            var errorMsg= MessageResourceContext.GetMessage(ErrorMessages.CanNotParseText, request);
            string? parsedText = await _textService.AddTermsTag(request.textToParse);
            return parsedText == null ? Result.Fail(new Error(errorMsg)) : Result.Ok(parsedText);
        }
    }
}
