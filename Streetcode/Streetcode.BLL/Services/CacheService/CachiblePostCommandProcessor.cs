using System.Text;
using System.Text.RegularExpressions;
using FluentResults;
using MediatR.Pipeline;

namespace Streetcode.BLL.Services.CacheService;

public class CachiblePostCommandProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : ICachibleCommandPostProcessor<TResponse>
{
    private readonly ICacheService _cacheService;
    public CachiblePostCommandProcessor(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (response!.ToString() !.Contains("IsSuccess='False'"))
        {
           return;
        }

        var resultString = GetEntityName(request.ToResult().ValueOrDefault.ToString() !);
        var sharedKeyForEntity = EraseLastCharIfThatEndLetterS(resultString);
        
        if (!await _cacheService.CacheKeyPatternExist(sharedKeyForEntity))
        {
            return;  
        }
        
        await _cacheService.InvalidateCacheAsync(sharedKeyForEntity);
    }

    private static string EraseLastCharIfThatEndLetterS(string input)
    {
        if (input.EndsWith("s"))
        {
            return input.Substring(0, input.Length - 1);
        }
            
        return input;
    }
    
    private static string GetEntityName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }
    
        const string commandSuffix = "Command";
    
        input = Regex.Replace(input, @"\{.*?\}", "").Trim();
    
        if (input.EndsWith(commandSuffix))
        {
            string trimmedInput = input.Substring(0, input.Length - commandSuffix.Length).Trim();
            StringBuilder result = new StringBuilder();
            foreach (char c in trimmedInput)
            {
                if (char.IsUpper(c) && result.Length > 0)
                {
                    result.Append(' ');
                }
    
                result.Append(c);
            }
    
            if (result.Length > 2)
            {
                List<string> resultList = result.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                resultList.RemoveAt(0);
                return string.Join("", resultList);
            }
            
            return result.ToString().Split(' ')[1];
        }
    
        return string.Empty;
    }
}
