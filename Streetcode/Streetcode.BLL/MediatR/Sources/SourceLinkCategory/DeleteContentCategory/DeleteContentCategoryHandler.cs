using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteContentCategory
{
    public class DeleteContentCategoryHandler : IRequestHandler<DeleteContentCategoryQuery, Result<StreetcodeCategoryContentDTO>>
    {
        private readonly IMapper m_mapper;
        private readonly ILoggerService m_logger;
        private readonly IRepositoryWrapper m_rep_wrapper;

        public DeleteContentCategoryHandler(IMapper mapper, ILoggerService logger, IRepositoryWrapper wrapper)
        {
            m_mapper = mapper;
            m_logger = logger;
            m_rep_wrapper = wrapper;
        }

        public async Task<Result<StreetcodeCategoryContentDTO>> Handle(DeleteContentCategoryQuery request, CancellationToken cancellationToken)
        {
            // FindStreetcode Content for removing
            var str_cont = await m_rep_wrapper.StreetcodeCategoryContentRepository
                .GetFirstOrDefaultAsync(e => e.SourceLinkCategoryId
                == request.sourcelinkcatId &&
            e.StreetcodeId == request.streetcodeId);

            if (str_cont == null)
            {
                m_logger.LogError(request, "StreetcodeCategory content for removing can't be found!");

                return Result.Fail("StreetcodeCategory content for removing can't be found!");
            }

            // Perform Delete operation
            m_rep_wrapper.StreetcodeCategoryContentRepository.Delete(str_cont);

            try
            {
                m_rep_wrapper.SaveChanges();
                return Result.Ok(m_mapper.Map<StreetcodeCategoryContentDTO>(str_cont));
            }
            catch (Exception e)
            {
                m_logger.LogError(request, $"Error occured while delete attempt. Error: {e.Message}");
                return Result.Fail($"Error occured while delete attempt. Error: {e.Message}");
            }
        }
    }
}
