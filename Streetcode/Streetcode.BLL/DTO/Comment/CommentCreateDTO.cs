using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Comment
{
    public class CommentCreateDTO
    {
        public string CommentContent { get; set; }
        public int StreetcodeId { get; set; }
    }
}
