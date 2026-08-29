using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Modules.Blog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRetryCountOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastError",
                schema: "blog",
                table: "outbox_messages",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextAttemptAt",
                schema: "blog",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "blog",
                table: "outbox_messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastError",
                schema: "blog",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "NextAttemptAt",
                schema: "blog",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "blog",
                table: "outbox_messages");
        }
    }
}
