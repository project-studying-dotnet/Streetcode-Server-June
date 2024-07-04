using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Enums.EnumExtensions;

namespace Streetcode.BLL.Mapping.Streetcode.TextContent;

public class TextProfile : Profile
{
    public TextProfile()
    {
        CreateMap<Text, TextDTO>().ReverseMap();
        CreateMap<TextCreateDTO, Text>().ReverseMap();

        CreateMap<Text, StreetcodeFilterResultDTO>()
            .ForMember(dest => dest.StreetcodeId, opt => opt.MapFrom(src => src.StreetcodeId))
            .ForMember(dest => dest.StreetcodeIndex, opt => opt.MapFrom(src => src.Streetcode!.Index))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.BlockName, opt => opt.MapFrom(src => SourceType.Text.GetDescription()))
            .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => SourceName.Text.GetDescription()));
    }
}
