using AutoMapper;
using Streetcode.BLL.DTO.Users;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.Mapping.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegisterDTO, User>()
                .ForPath(
                dest => dest.UserName, conf => conf.MapFrom(src => src.Username))                            
                .ForPath(
                dest => dest.FirstName, conf => conf.MapFrom(src => src.FirstName))
                .ForPath(
                dest => dest.LastName, conf => conf.MapFrom(src => src.LastName))
                .ForPath(
                dest => dest.BirthDate, conf => conf.MapFrom(src => src.Birthday))
                .ForPath(
                dest => dest.Email, conf => conf.MapFrom(src => src.Email))
                .ForPath(
                dest => dest.PhoneNumber, conf => conf.MapFrom(src => src.Phone))
                .ReverseMap();

            CreateMap<UserDTO, User>()
                .ForPath(
                dest => dest.UserName, conf => conf.MapFrom(src => src.Username))
                .ForPath(
                dest => dest.FirstName, conf => conf.MapFrom(src => src.FirstName))
                .ForPath(
                dest => dest.LastName, conf => conf.MapFrom(src => src.LastName))
                .ForPath(
                dest => dest.BirthDate, conf => conf.MapFrom(src => src.Birthday))
                .ForPath(
                dest => dest.Email, conf => conf.MapFrom(src => src.Email))
                .ForPath(
                dest => dest.PhoneNumber, conf => conf.MapFrom(src => src.Phone))
                .ForPath(
                dest => dest.EmailConfirmed, conf => conf.MapFrom(src => src.EmailConfirmed))
                .ForPath(
                dest => dest.PhoneNumberConfirmed, conf => conf.MapFrom(src => src.PhoneConfirmed))
                .ForPath(
                dest => dest.TwoFactorEnabled, conf => conf.MapFrom(src => src.TwoFactorEnabled))
                .ReverseMap();
        }
    }
}
