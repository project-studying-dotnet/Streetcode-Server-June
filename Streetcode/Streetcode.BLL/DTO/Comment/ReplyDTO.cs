using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Comment
{
    public class ReplyDTO : CommentDTO
    {
        public int ParentId { get; set; }
    }
}
