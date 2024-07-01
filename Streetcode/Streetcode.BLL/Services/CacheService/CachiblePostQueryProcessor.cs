using FluentResults;
using MediatR.Pipeline;
using Newtonsoft.Json.Linq;

namespace Streetcode.BLL.Services.CacheService
{
    public class CachiblePostQueryProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : ICachibleQueryBehavior<TResponse>
    {
        private readonly ICacheService _cacheService;

        public CachiblePostQueryProcessor(ICacheService cacheService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (response!.ToString() !.Contains("IsSuccess='False'"))
            {
                return;
            }
            
            var validKey = CreateValidKey(request);
            if (string.IsNullOrEmpty(await validKey))
            {
                return;
            }
            
            var valueToken = JObject.FromObject(response)["Value"];
            if (valueToken != null)
            {
                var value = valueToken.ToString();
                await _cacheService.SetCacheAsync(await validKey, value);
            }
        }
        
        private async Task<string> CreateValidKey(TRequest request)
        {
            var cacheKey = request.ToResult().ValueOrDefault?.ToString();
            if (string.IsNullOrEmpty(cacheKey))
            {
                return string.Empty;
            }

            string validKey;
            if (cacheKey.Contains(','))
            {
                var keyValuePairs = cacheKey.Substring(0, cacheKey.IndexOf(',')).Split(',');
                validKey = keyValuePairs[0].Insert(keyValuePairs[0].Length, "}");
            }
            else
            {
                validKey = cacheKey.Split('{')[0].Trim();
            }

            if (await _cacheService.CacheKeyIsExist(validKey))
            {
                return string.Empty;
            }

            return validKey;
        }
    }
}
