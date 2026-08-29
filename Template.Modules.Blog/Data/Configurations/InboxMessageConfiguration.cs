using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Modules.Blog.Data.Inbox;

namespace Template.Modules.Blog.Data.Configurations;

internal sealed class InboxMessageConfiguration
    : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(
        EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages");

        builder.HasKey(x => x.MessageId);

        builder.Property(x => x.ProcessedAt)
            .IsRequired();
    }
}