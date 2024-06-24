using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Specification.Streetcode.TextSpec.GetById;

public class TextByIdSpec : Specification<Text>
{
    public TextByIdSpec(int textId)
    {
        Query.Where(st => st.Id == textId);
    }
}
