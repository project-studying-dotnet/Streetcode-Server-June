using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Mapping.Media.Images;

public class StreetcodeArtProfile : Profile
{
    public StreetcodeArtProfile()
    {
        CreateMap<StreetcodeArt, StreetcodeArtDTO>().ReverseMap();
        CreateMap<StreetcodeArt, StreetcodeFilterResultDTO>()
            .ForMember(dest => dest.StreetcodeId, opt => opt.MapFrom(src => src.StreetcodeId))
            .ForMember(dest => dest.StreetcodeIndex, opt => opt.MapFrom(src => src.Streetcode!.Index))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Art!.Title))
            .ForMember(dest => dest.BlockName, opt => opt.MapFrom("art-gallery"))
            .ForMember(dest => dest.SourceName, opt => opt.MapFrom("Арт-галерея"));
    }
}
