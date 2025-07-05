using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kyoushitsu.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create extension if not exists citext;");
            
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "posts",
                type: "citext",
                nullable: false);
            
            migrationBuilder.CreateIndex(name: "slug", table: "posts", column: "slug", unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "slug",
                table: "posts");
        }
    }
}
