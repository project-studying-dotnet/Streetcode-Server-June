using AutoMapper;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Mapping.Streetcode.TextContent;

public class RelatedTermProfile : Profile
{
    public RelatedTermProfile()
    {
        CreateMap<RelatedTerm, RelatedTermDTO>().ReverseMap();
        CreateMap<RelatedTermCreateDTO, RelatedTerm>().ReverseMap();
    }
}
