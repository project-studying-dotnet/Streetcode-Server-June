using FluentValidation;
using MediatR;

namespace Streetcode.BLL.Behavior;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IValidatableRequest<TResponse>
{
    private readonly IValidator<TRequest> _validators;

    public ValidationBehavior(IValidator<TRequest> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await _validators.ValidateAndThrowAsync(request, cancellationToken);
        return await next();
    }
}