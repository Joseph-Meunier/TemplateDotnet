using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.Modules.Blog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tags",
                schema: "blog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "post_tags",
                schema: "blog",
                columns: table => new
                {
                    PostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_tags", x => new { x.PostsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_post_tags_posts_PostsId",
                        column: x => x.PostsId,
                        principalSchema: "blog",
                        principalTable: "posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_post_tags_tags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "blog",
                        principalTable: "tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_post_tags_TagsId",
                schema: "blog",
                table: "post_tags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_tags_Name",
                schema: "blog",
                table: "tags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_tags",
                schema: "blog");

            migrationBuilder.DropTable(
                name: "tags",
                schema: "blog");
        }
    }
}
