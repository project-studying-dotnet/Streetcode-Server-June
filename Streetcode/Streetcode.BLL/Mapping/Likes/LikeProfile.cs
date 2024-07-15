using AutoMapper;
using Streetcode.BLL.DTO.Likes;
using Streetcode.DAL.Entities.Likes;

namespace Streetcode.BLL.Mapping.Likes
{
    public class LikeProfile : Profile
    {
        public LikeProfile()
        {
            CreateMap<Like, LikeDTO>().ReverseMap();
            CreateMap<PushLikeDTO, LikeDTO>().ReverseMap();
            CreateMap<PushLikeDTO, Like>().ReverseMap();
        }
    }
}
