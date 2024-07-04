using FluentValidation;
namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;

public class StreetcodeFilterRequestDtoValidator : AbstractValidator<GetStreetcodeByFilterQuery>
{
    public StreetcodeFilterRequestDtoValidator()
    {
        RuleFor(x => x.Filter.SearchQuery).NotNull().NotEmpty();
    }
}
