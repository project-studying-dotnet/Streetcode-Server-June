using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Util
{
    public class IdComparerTerm : IEqualityComparer<Term>
    {
        public bool Equals(Term? x, Term? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Term obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
