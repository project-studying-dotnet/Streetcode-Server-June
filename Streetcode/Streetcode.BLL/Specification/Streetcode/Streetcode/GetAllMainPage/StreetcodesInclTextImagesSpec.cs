using Ardalis.Specification;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.GetAllMainPage;

public class StreetcodesInclTextImagesSpec : Specification<StreetcodeContent>
{
    public StreetcodesInclTextImagesSpec() 
    {
        Query.Where(sc => sc.Status == DAL.Enums.StreetcodeStatus.Published)
            .Include(sc => sc.Texts)
            .Include(sc => sc.Images);    
    }
}
