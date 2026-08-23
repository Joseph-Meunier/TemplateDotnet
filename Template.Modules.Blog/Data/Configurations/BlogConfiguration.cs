using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Modules.Blog.Domain;

namespace Template.Modules.Blog.Data.Configurations;

public sealed class BlogConfiguration
    : IEntityTypeConfiguration<Post>
{
    public void Configure(
        EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("blog_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}