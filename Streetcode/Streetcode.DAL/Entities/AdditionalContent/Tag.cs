using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.AdditionalContent;

public class Tag
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public IEnumerable<StreetcodeTagIndex>? StreetcodeTagIndices { get; set; }

    public IEnumerable<StreetcodeContent>? Streetcodes { get; set; }
}