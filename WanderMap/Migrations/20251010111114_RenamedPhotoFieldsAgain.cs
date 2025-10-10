using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WanderMap.Migrations
{
    /// <inheritdoc />
    public partial class RenamedPhotoFieldsAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobPath",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Photos",
                newName: "OriginalFileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "Photos",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "BlobPath",
                table: "Photos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
