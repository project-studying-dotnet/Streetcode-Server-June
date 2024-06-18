using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streetcode.DAL.Persistence.Migrations
{
    public partial class UpdatedFactAndTextMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_texts_StreetcodeId",
                schema: "streetcode",
                table: "texts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "streetcode",
                table: "texts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                schema: "streetcode",
                table: "texts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                schema: "streetcode",
                table: "texts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                schema: "streetcode",
                table: "facts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_texts_StreetcodeId",
                schema: "streetcode",
                table: "texts",
                column: "StreetcodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_texts_StreetcodeId",
                schema: "streetcode",
                table: "texts");

            migrationBuilder.DropColumn(
                name: "Author",
                schema: "streetcode",
                table: "texts");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                schema: "streetcode",
                table: "texts");

            migrationBuilder.DropColumn(
                name: "Position",
                schema: "streetcode",
                table: "facts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "streetcode",
                table: "texts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_texts_StreetcodeId",
                schema: "streetcode",
                table: "texts",
                column: "StreetcodeId",
                unique: true);
        }
    }
}
