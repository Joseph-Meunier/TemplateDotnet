using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Modules.Blog.Domain;

namespace Template.Modules.Blog.Data.Configurations;

public sealed class PostConfiguration
    : IEntityTypeConfiguration<Post>
{
    public void Configure(
        EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("posts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.ReadingTimeMinutes)
            .IsRequired();

        builder.Property(x => x.AuthorUserId)
            .IsRequired();
    }
}