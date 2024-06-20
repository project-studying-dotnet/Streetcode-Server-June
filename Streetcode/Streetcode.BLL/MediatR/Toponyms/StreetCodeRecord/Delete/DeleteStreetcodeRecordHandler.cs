﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Toponyms.StreetCodeRecord.Delete
{
    public class DeleteStreetcodeRecordHandler : IRequestHandler<DeleteStreetcodeRecordQuery, Result<StreetcodeRecordDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteStreetcodeRecordHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<StreetcodeRecordDTO>> Handle(DeleteStreetcodeRecordQuery request, CancellationToken cancellationToken)
        {
            var record = await _repositoryWrapper.StreetcodeToponymRepository.GetFirstOrDefaultAsync(p => p.StreetcodeId == request.StreetcodeId &&
                                                                                                          p.ToponymId == request.ToponymId);
            if (record == null)
            {
                var errorMsgNull = MessageResourceContext.GetMessage(ErrorMessages.FailToConvertNull, request);
                _logger.LogError(request, errorMsgNull);
                return Result.Fail(new Error(errorMsgNull));
            }

            _repositoryWrapper.StreetcodeToponymRepository.Delete(record);
            _repositoryWrapper.SaveChanges();

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                const string errorMsg = "Failed to create a team";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<StreetcodeRecordDTO>(record));
        }
    }
}
