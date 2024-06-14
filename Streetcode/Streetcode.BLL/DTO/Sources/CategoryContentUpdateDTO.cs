using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Sources
{
    public class CategoryContentUpdateDTO
    {
        public int SourceLinkCategoryId { get; set; }
        [MaxLength(4000)]
        public string Text { get; set; } = string.Empty;
        public int StreetcodeId { get; set; }
    }
}
