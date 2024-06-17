using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Partners;

public class Partner
{
    public int Id { get; set; }

    public string Title { get; set; }

    public int LogoId { get; set; }

    public bool IsKeyPartner { get; set; }

    public bool IsVisibleEverywhere { get; set; }

    public string? TargetUrl { get; set; }

    public string? UrlTitle { get; set; }

    public string? Description { get; set; }

    public Image? Logo { get; set; }

    public List<PartnerSourceLink> PartnerSourceLinks { get; set; } = new ();

    public List<StreetcodeContent> Streetcodes { get; set; } = new ();
}