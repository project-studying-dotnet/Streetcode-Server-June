﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Term.DeleteById
{
    public class DeleteTermByIdHandler : IRequestHandler<DeleteTermByIdCommand, Result<TermDTO>>
    {
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public DeleteTermByIdHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TermDTO>> Handle(DeleteTermByIdCommand request, CancellationToken cancellationToken)
        {
            var term = await _repository.TermRepository.GetFirstOrDefaultAsync(t => t.Id == request.Id);

            if (term is null)
            {
                var errorMsg = $"Cannot find a term: {request.Id}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repository.TermRepository.Delete(term);

            var resultIsSuccess = await _repository.SaveChangesAsync() > 0;
            var relatedTermDto = _mapper.Map<TermDTO>(term);
            if(resultIsSuccess && relatedTermDto != null)
            {
                return Result.Ok(relatedTermDto);
            }
            else
            {
                const string errorMsg = "Failed to delete a related term";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
