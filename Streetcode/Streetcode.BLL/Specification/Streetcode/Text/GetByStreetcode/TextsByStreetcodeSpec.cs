using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextSpec.GetByStreetcode;

public class TextsByStreetcodeSpec : Specification<Text>
{
    public TextsByStreetcodeSpec(int streetcodeId)
    {
        Query.Where(text => text.StreetcodeId == streetcodeId);
    }  
}
