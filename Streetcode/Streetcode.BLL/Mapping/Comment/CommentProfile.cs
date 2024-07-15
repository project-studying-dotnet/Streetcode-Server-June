using AutoMapper;
using Streetcode.BLL.DTO.Comment;
using Streetcode.DAL.Entities.Comments;

namespace Streetcode.BLL.Mapping.Comments
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CommentCreateDTO, Comment>().ReverseMap();
            CreateMap<CommentDTO, Comment>().ReverseMap();
            CreateMap<ReplyCreateDTO, Comment>().ReverseMap();
            CreateMap<ReplyDTO, Comment>().ReverseMap();
        }
    }
}
