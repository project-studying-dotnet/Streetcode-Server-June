using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Mapping.Media.Images.Resolvers;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Enums.EnumExtensions;

namespace Streetcode.BLL.Mapping.Media.Images;

public class ArtProfile : Profile
{
    public ArtProfile()
    {
        CreateMap<Art, ArtDTO>().ReverseMap();
        CreateMap<Art, ArtCreateUpdateDTO>().ReverseMap();

        CreateMap<Art, StreetcodeFilterResultDTO>()
             .ForMember(dest => dest.StreetcodeId, opt => opt.MapFrom<StreetcodeIdResolver>())
             .ForMember(dest => dest.StreetcodeIndex, opt => opt.MapFrom<StreetcodeIndexResolver>())
             .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Title))
             .ForMember(dest => dest.BlockName, opt => opt.MapFrom(src => SourceType.ArtGallery.GetDescription()))
             .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => SourceName.ArtGallery.GetDescription()));
    }
}
