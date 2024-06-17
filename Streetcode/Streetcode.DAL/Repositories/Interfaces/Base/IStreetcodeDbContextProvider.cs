using Streetcode.DAL.Persistence;

namespace Streetcode.DAL.Repositories.Interfaces.Base
{
    public interface IStreetcodeDbContextProvider
    {
        StreetcodeDbContext DbContext { init; }
    }
}
