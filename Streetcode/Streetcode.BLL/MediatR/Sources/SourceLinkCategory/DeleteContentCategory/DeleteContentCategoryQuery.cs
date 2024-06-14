using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteContentCategory
{
    public record DeleteContentCategoryQuery(int sourcelinkcatId, int streetcodeId) : IRequest<Result<StreetcodeCategoryContentDTO>>;
}
