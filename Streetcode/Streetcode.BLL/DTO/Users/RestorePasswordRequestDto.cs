using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Users
{
    public class RestorePasswordRequestDto
    {
        // email for verification
        public string? Email { get; set; }
    }
}
