using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Toponyms;

public class Toponym
{
    public int Id { get; set; }

    public string Oblast { get; set; } = string.Empty;

    public string AdminRegionOld { get; set; } = string.Empty;

    public string AdminRegionNew { get; set; } = string.Empty;

    public string Gromada { get; set; } = string.Empty;

    public string Community { get; set; } = string.Empty;

    public string StreetName { get; set; } = string.Empty;

    public string StreetType { get; set; } = string.Empty;

    public List<StreetcodeContent> Streetcodes { get; set; } = new ();

    public ToponymCoordinate Coordinate { get; set; } = new();
}