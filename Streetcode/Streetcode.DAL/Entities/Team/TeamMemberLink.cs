using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Entities.Team
{
    public class TeamMemberLink
    {
        public int Id { get; set; }

        public LogoType LogoType { get; set; }

        public string? TargetUrl { get; set; }

        public int TeamMemberId { get; set; }

        public TeamMember? TeamMember { get; set; }
    }
}