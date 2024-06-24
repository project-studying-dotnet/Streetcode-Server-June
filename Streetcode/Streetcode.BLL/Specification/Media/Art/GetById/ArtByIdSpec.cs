using Ardalis.Specification;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Specification.Media.ArtSpec.GetById;

public class ArtByIdSpec : Specification<Art>
{
    public ArtByIdSpec(int artId)
    {
        Query.Where(f => f.Id == artId);
    }
}
