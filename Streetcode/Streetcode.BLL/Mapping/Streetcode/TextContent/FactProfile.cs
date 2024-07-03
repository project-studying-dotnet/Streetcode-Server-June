using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Mapping.Streetcode.TextContent;

public class FactProfile : Profile
{
    public FactProfile()
    {
        CreateMap<Fact, FactDto>().ReverseMap();
        CreateMap<Fact, FactUpdateCreateDto>().ReverseMap();
        CreateMap<Fact, StreetcodeFilterResultDTO>()
            .ForMember(dest => dest.StreetcodeId, opt => opt.MapFrom(src => src.StreetcodeId))
            .ForMember(dest => dest.StreetcodeIndex, opt => opt.MapFrom(src => src.Streetcode!.Index))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.BlockName, opt => opt.MapFrom("wow-facts"))
            .ForMember(dest => dest.SourceName, opt => opt.MapFrom("Wow-факти"));
    }
}
