using System.ComponentModel.DataAnnotations;

namespace Streetcode.DAL.Entities.Streetcode.Types;

public class PersonStreetcode : StreetcodeContent
{
    public string FirstName { get; set; }

    public string? Rank { get; set; }

    public string LastName { get; set; }
}