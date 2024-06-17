using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Toponyms;

namespace Streetcode.DAL.Persistence.Configurations.Toponyms
{
    internal class StreetcodeToponymConfiguration : IEntityTypeConfiguration<StreetcodeToponym>
    {
        public void Configure(EntityTypeBuilder<StreetcodeToponym> builder)
        {
        }
    }
}
