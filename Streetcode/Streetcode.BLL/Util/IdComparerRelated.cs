using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Util
{
    public class IdComparerRelated : IEqualityComparer<RelatedTerm>
    {
        public bool Equals(RelatedTerm? x, RelatedTerm? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] RelatedTerm obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
