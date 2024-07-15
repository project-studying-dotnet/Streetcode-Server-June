namespace Streetcode.BLL.DTO.Comment;

public class ReplyCreateDTO : CommentCreateDTO
{
    public int ParentId { get; set; }
}