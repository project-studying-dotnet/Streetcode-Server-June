using MediatR;

namespace Streetcode.BLL.Behavior;

public interface IValidatableRequest<TResponse> : IRequest<TResponse>, IValidatableRequest
{
}

public interface IValidatableRequest
{
}