using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.Toponyms;

public class ToponymDTO
{
    public int Id { get; set; }
    public string Oblast { get; set; } = string.Empty;
    public string AdminRegionOld { get; set; } = string.Empty;
    public string AdminRegionNew { get; set; } = string.Empty;
    public string Gromada { get; set; } = string.Empty;
    public string Community { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string StreetType { get; set; } = string.Empty;

    public ToponymCoordinateDTO Coordinate { get; set; } = new();
    public IEnumerable<StreetcodeDTO>? Streetcodes { get; set; }
}