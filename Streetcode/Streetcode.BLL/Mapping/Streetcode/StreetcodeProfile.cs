using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Util;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.Mapping.Streetcode;

public class StreetcodeProfile : Profile
{
    public StreetcodeProfile()
    {
        CreateMap<StreetcodeContent, StreetcodeDTO>()
            .ForMember(x => x.StreetcodeType, conf => conf.MapFrom(s => GetStreetcodeType(s)))
            .ReverseMap();
        CreateMap<StreetcodeContent, StreetcodeShortDTO>().ReverseMap();
        CreateMap<StreetcodeContent, StreetcodeMainPageDTO>()
             .ForPath(dto => dto.Text, conf => conf
                .MapFrom(e => e.Texts.Select(t => t.Title).ToList()))
            .ForPath(dto => dto.ImageId, conf => conf
                .MapFrom(e => e.Images.Select(i => i.Id).LastOrDefault()));

        CreateMap<CreateStreetcodeDTO, StreetcodeContent>()
            .ConstructUsing((dto, sc) => dto.StreetcodeType
            switch
            {
                StreetcodeType.Event => new EventStreetcode(),
                StreetcodeType.Person => new PersonStreetcode()
                {
                    FirstName = dto.FirstName!,
                    LastName = dto.LastName!,
                    Rank = dto.Rank
                },
                _ => new StreetcodeContent(),
            })
            .ForMember(
            x => x.DateString,
            y => y.MapFrom(
                dto => DateToStringConverter
                .CreateDateString(
                    dto.EventStartOrPersonBirthDate, dto.EventEndOrPersonDeathDate)));

        CreateMap<EventStreetcode, CreateStreetcodeDTO>();
        CreateMap<PersonStreetcode, CreateStreetcodeDTO>();

        CreateMap<StreetcodeContent, StreetcodeFilterResultDTO>()
            .ForMember(dest => dest.StreetcodeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StreetcodeTransliterationUrl, opt => opt.MapFrom(src => src.TransliterationUrl))
            .ForMember(dest => dest.StreetcodeIndex, opt => opt.MapFrom(src => src.Index))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Title));
    }

    private StreetcodeType GetStreetcodeType(StreetcodeContent streetcode)
    {
        if(streetcode is EventStreetcode)
        {
            return StreetcodeType.Event;
        }

        return StreetcodeType.Person;
    }
}
