using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Modules.Users.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndentityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                schema: "users",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_users_IdentityId",
                schema: "users",
                table: "users",
                column: "IdentityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_IdentityId",
                schema: "users",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                schema: "users",
                table: "users");
        }
    }
}
