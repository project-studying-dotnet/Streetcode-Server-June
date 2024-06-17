using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.DAL.Entities.Team
{
    public class Positions
    {
        public int Id { get; set; }

        public string? Position { get; set; }

        public List<TeamMember>? TeamMembers { get; set; }
    }
}
