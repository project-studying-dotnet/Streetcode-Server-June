using AutoMapper;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.Types;

namespace Streetcode.BLL.Mapping.Streetcode;

public class RelatedFigureProfile : Profile
{
    public RelatedFigureProfile()
    {
        CreateMap<EventStreetcode, RelatedFigureDTO>()
            .ForPath(dto => dto.Title, conf => conf
                .MapFrom(e => e.Title))
            .ForPath(dto => dto.Url, conf => conf
                .MapFrom(e => e.TransliterationUrl));

        CreateMap<PersonStreetcode, RelatedFigureDTO>()
            .ForPath(dto => dto.Url, conf => conf
                .MapFrom(e => e.TransliterationUrl));

        CreateMap<StreetcodeContent, RelatedFigureShortDTO>();
    }
}
