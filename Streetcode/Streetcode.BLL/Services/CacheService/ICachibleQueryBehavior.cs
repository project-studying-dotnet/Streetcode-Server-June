using MediatR;

namespace Streetcode.BLL.Services.CacheService;

public interface ICachibleQueryBehavior<TResult> : IRequest<TResult>
{
    TResult? CachedResponse { get; set; }
}
