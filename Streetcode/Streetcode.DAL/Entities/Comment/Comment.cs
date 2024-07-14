using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streetcode.DAL.Entities.Base;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.DAL.Entities.Comments
{
    public class Comment : IEntityId<int>
    {
        public int Id { get; set; }
        public int? StreetcodeId { get; set; }
        public Guid? UserId { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public User? User { get; set; }
        public StreetcodeContent? Streetcode { get; set; }
        public int? ParentId { get; set; }
        public Comment? ParentComment { get; set; }
        public List<Comment> Replies { get; set; } = new();
    }
}
