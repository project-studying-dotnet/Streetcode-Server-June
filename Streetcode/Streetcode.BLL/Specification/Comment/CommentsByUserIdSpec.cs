using Ardalis.Specification;
using Streetcode.DAL.Entities.Comments;

namespace Streetcode.BLL.Specification.Comment
{
    public class CommentsByUserIdSpec : Specification<DAL.Entities.Comments.Comment>
    {
        public CommentsByUserIdSpec(Guid userId) 
        {
            Query.Where(f => f.UserId == userId);
        }
    }
}
