using AutoMapper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.DAL.Entities.Media;

namespace Streetcode.BLL.Mapping.Media;

public class AudioProfile : Profile
{
    public AudioProfile()
    {
        CreateMap<Audio, AudioDTO>()
            .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType));

        CreateMap<AudioFileBaseCreateDTO, Audio>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType));
	}
}
