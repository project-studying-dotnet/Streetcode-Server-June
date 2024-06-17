using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Feedback;

namespace Streetcode.DAL.Persistence.Configurations.Feedback
{
    internal class ResponseConfiguration : IEntityTypeConfiguration<Response>
    {
        public void Configure(EntityTypeBuilder<Response> builder)
        {
            builder.ToTable("responses", "feedback");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            builder.Property(r => r.Name).HasMaxLength(50);

            builder.Property(r => r.Email)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Description).HasMaxLength(1000);
        }
    }
}
