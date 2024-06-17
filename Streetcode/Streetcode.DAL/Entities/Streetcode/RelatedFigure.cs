using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.DAL.Entities.Streetcode;

public class RelatedFigure
{
    public int ObserverId { get; set; }

    public StreetcodeContent Observer { get; set; } = new();

    public int TargetId { get; set; }

    public StreetcodeContent Target { get; set; } = new();
}