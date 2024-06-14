using Streetcode.BLL.DTO.AdditionalContent;

namespace Streetcode.BLL.DTO.Transactions;

public class TransactLinkDTO
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public int StreetcodeId { get; set; }
}