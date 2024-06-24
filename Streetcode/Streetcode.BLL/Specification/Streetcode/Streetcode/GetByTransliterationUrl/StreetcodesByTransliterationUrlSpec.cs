using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetByTransliterationUrl;

public class StreetcodesByTransliterationUrlSpec : Specification<StreetcodeContent>
{
    public StreetcodesByTransliterationUrlSpec(string url)
    {
        Query.Where(st => st.TransliterationUrl == url);
    }
}
