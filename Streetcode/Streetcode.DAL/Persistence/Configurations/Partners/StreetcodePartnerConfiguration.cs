using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Partners;

namespace Streetcode.DAL.Persistence.Configurations.Partners
{
    internal class StreetcodePartnerConfiguration : IEntityTypeConfiguration<StreetcodePartner>
    {
        public void Configure(EntityTypeBuilder<StreetcodePartner> builder)
        {
        }
    }
}
