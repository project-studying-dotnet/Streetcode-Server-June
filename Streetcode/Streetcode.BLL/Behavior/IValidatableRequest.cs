using MediatR;

namespace Streetcode.BLL.Behavior;

public interface IValidatableRequest<out TResponse> : IRequest<TResponse>, IValidatableRequest
{
}

public interface IValidatableRequest
{
}