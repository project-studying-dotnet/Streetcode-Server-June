using MediatR;

namespace Streetcode.BLL.Services.CacheService;

public interface ICachibleCommandPostProcessor<out TResponse> : IRequest<TResponse>
{
}