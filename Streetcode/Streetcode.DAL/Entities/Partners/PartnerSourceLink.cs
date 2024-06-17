using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Entities.Partners;

public class PartnerSourceLink
{
    public int Id { get; set; }

    public LogoType LogoType { get; set; }

    public string? TargetUrl { get; set; }

    public int PartnerId { get; set; }

    public Partner? Partner { get; set; }
}