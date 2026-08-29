using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Modules.Blog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFailedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FailedAt",
                schema: "blog",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedAt",
                schema: "blog",
                table: "outbox_messages");
        }
    }
}
