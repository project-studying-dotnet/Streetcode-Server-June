using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Text.Json.Serialization;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete
{
    public class DeleteFactHandler : IRequestHandler<DeleteFactCommand, Result<FactDto>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IDistributedCache m_distributedCache;

        public DeleteFactHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger, IMapper mapper, IDistributedCache distributedCache)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
            _mapper = mapper;
            m_distributedCache = distributedCache;
        }

        public async Task<Result<FactDto>> Handle(DeleteFactCommand request, CancellationToken cancellationToken)
        {
            string key = "FactDTO";

            var fact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);
            if(fact == null)
            {
                const string errorMsg = "Cannot find a fact with this id";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.FactRepository.Delete(fact);
            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
            if(resultIsSuccess)
            {
                var oldCache = await m_distributedCache.GetStringAsync(key);

                // Some old cache was found
                if(oldCache != null) 
                {
                    List<FactDto>? des_old_cache = JsonConvert.DeserializeObject<IEnumerable<FactDto>>(
                        oldCache, new JsonSerializerSettings()
                        { NullValueHandling = NullValueHandling.Ignore }).ToList();

                    if (des_old_cache != null)
                    {
                        int rem_index = -1;

                        des_old_cache.ForEach(
                            f =>
                            { 
                                if(f.Id)
                            });

                        if (rem_index >= 0)
                        {
                            des_old_cache.ToList().RemoveAt(rem_index);
                        }
                    }                    
                }

                return Result.Ok(_mapper.Map<FactDto>(fact));
            }
            else
            {
                const string errorMsg = $"Failed to delete fact";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
