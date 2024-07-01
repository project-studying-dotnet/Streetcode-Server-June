using FluentResults;
using MediatR;
using Newtonsoft.Json;

namespace Streetcode.BLL.Services.CacheService
{
    public class CachibleQueryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICachibleQueryBehavior<TResponse>
    {
        private readonly ICacheService _cacheService;

        public CachibleQueryBehavior(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var key = request.ToResult().Value.ToString() !;
            var cachedValue = await _cacheService.GetCacheAsync(key);

            if (!string.IsNullOrEmpty(cachedValue))
            {
                var innerType = typeof(TResponse).GetGenericArguments()[0];
                var cachedObject = JsonConvert.DeserializeObject(cachedValue, innerType);
                var resultType = typeof(Result<>).MakeGenericType(innerType);
                var resultInstance = Activator.CreateInstance(resultType);
                var successProperty = resultType.GetProperty("Value");
                successProperty?.SetValue(resultInstance, cachedObject);
                request.CachedResponse = (TResponse)resultInstance!;
                return await next();
            }
            
            return await next();
        }
    }
}
