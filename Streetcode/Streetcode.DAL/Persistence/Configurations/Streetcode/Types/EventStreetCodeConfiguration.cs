using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Streetcode.Types;

namespace Streetcode.DAL.Persistence.Configurations.Streetcode.Types
{
    internal class EventStreetCodeConfiguration : IEntityTypeConfiguration<EventStreetcode>
    {
        public void Configure(EntityTypeBuilder<EventStreetcode> builder)
        {
        }
    }
}
