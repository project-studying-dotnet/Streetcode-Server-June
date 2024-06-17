using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.DAL.Entities.AdditionalContent.Coordinates;

public class Coordinate
{
    public int Id { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longtitude { get; set; }
}