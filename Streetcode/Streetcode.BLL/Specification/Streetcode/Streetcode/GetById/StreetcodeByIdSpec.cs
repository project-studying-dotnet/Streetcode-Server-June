using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetById;

public class StreetcodeByIdSpec : Specification<StreetcodeContent>
{
    public StreetcodeByIdSpec(int streetcodeId)
    {
        Query.Where(st => st.Id == streetcodeId);
    }
}
