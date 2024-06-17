using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Transactions;

namespace Streetcode.DAL.Persistence.Configurations.Transactions
{
    internal class TransactionLinkConfiguration : IEntityTypeConfiguration<TransactionLink>
    {
        public void Configure(EntityTypeBuilder<TransactionLink> builder)
        {
            builder.ToTable("transaction_links", "transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.UrlTitle).HasMaxLength(255);

            builder.Property(t => t.Url)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
