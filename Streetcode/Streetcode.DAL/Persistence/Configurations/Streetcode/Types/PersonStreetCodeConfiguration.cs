using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Streetcode.Types;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode.Types
{
    internal class PersonStreetCodeConfiguration : IEntityTypeConfiguration<PersonStreetcode>
    {
        public void Configure(EntityTypeBuilder<PersonStreetcode> builder)
        {
            builder.Property(p => p.FirstName).HasMaxLength(50);

            builder.Property(p => p.Rank).HasMaxLength(50);

            builder.Property(p => p.LastName).HasMaxLength(50);
        }
    }
}
