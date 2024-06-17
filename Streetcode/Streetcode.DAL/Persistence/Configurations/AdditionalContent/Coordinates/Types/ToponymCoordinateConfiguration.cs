using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Persistence.Configurations.AdditionalContent.Coordinates.Types
{
    internal class ToponymCoordinateConfiguration : IEntityTypeConfiguration<ToponymCoordinate>
    {
        public void Configure(EntityTypeBuilder<ToponymCoordinate> builder)
        {
        }
    }
}
