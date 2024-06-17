using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.DAL.Persistence.Configurations.Users
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Surname)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email).IsRequired();

            builder.Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.Role).IsRequired();
        }
    }
}
