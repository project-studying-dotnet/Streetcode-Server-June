using Streetcode.BLL.DTO.AdditionalContent;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.Partners
{
    public class CreatePartnerDTO
    {
        public int Id { get; set; }
        public bool IsKeyPartner { get; set; }
        public bool IsVisibleEverywhere { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public int LogoId { get; set; }
        public string UrlTitle { get; set; } = string.Empty;
        public List<CreatePartnerSourceLinkDTO>? PartnerSourceLinks { get; set; }
        public List<StreetcodeShortDTO> Streetcodes { get; set; } = new List<StreetcodeShortDTO>();
    }
}
