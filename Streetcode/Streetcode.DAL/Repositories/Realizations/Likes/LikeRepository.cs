using Streetcode.DAL.Entities.Likes;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Likes;
using Streetcode.DAL.Repositories.Realizations.Base;

namespace Streetcode.DAL.Repositories.Realizations.Likes
{
    public class LikeRepository : RepositoryBase<Like>, ILikeRepository
    {
        public LikeRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }

        public LikeRepository()
        {
        }
    }
}
