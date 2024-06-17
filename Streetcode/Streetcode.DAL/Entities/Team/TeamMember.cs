using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Entities.Team
{
    public class TeamMember
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Description { get; set; }

        public bool IsMain { get; set; }

        public List<TeamMemberLink>? TeamMemberLinks { get; set; }

        public List<Positions>? Positions { get; set; }

        public int ImageId { get; set; }

        public Image? Image { get; set; }
    }
}
