using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.Partners;

public class PartnerDTO
{
    public int Id { get; set; }
    public bool IsKeyPartner { get; set; }
    public bool IsVisibleEverywhere { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LogoId { get; set; }
    public UrlDTO TargetUrl { get; set; } = new();
    public List<PartnerSourceLinkDTO> PartnerSourceLinks { get; set; } = new();
    public List<StreetcodeShortDTO> Streetcodes { get; set; } = new();
}