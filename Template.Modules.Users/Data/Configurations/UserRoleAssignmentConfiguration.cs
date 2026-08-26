using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Modules.Users.Domain;

namespace Template.Modules.Users.Data.Configurations;

public sealed class UserRoleAssignmentConfiguration
    : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(
        EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(x => new
        {
            x.UserId,
            x.Role
        });

        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}