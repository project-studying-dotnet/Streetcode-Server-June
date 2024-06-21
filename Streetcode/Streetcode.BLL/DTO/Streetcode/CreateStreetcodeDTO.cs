using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Streetcode
{
    public class CreateStreetcodeDTO
    {
        public int Index { get; set; }
        public StreetcodeType StreetcodeType { get; set; } = StreetcodeType.Person;
        public string Title { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? Rank { get; init; }
        public string? LastName { get; set; }
        public DateTime EventStartOrPersonBirthDate { get; init; }
        public DateTime? EventEndOrPersonDeathDate { get; init; }
        public IEnumerable<int> TagIds { get; init; } = Array.Empty<int>();
        public string? Teaser { get; set; }
        public IEnumerable<int> ImageIds { get; init; } = Array.Empty<int>();
        public string? InstagramARLink { get; set; }
        public string? InvolvedPeople { get; set; }
        public int? AudioId { get; init; }
        public string TransliterationUrl { get; set; } = string.Empty;
        public string? Alias { get; set; }
    }
}
