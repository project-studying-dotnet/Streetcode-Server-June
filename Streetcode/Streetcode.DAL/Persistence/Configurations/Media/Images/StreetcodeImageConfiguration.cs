using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Persistence.Configurations.Media.Images
{
    internal class StreetcodeImageConfiguration : IEntityTypeConfiguration<StreetcodeImage>
    {
        public void Configure(EntityTypeBuilder<StreetcodeImage> builder)
        {
        }
    }
}
