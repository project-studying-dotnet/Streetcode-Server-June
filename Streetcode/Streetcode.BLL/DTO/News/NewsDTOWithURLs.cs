using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.News
{
    public class NewsDTOWithURLs
    {
        public NewsDTO News { get; set; } = new NewsDTO();

        public string PrevNewsUrl { get; set; } = string.Empty;

        public string NextNewsUrl { get; set; } = string.Empty;

        public RandomNewsDTO? RandomNews { get; set; } = new RandomNewsDTO();
    }
}
