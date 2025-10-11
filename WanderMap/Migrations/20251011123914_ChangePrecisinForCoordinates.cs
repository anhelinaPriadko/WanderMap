using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WanderMap.Migrations
{
    /// <inheritdoc />
    public partial class ChangePrecisinForCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Places",
                type: "numeric(18,12)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(9,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Places",
                type: "numeric(18,12)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(9,6)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Places",
                type: "numeric(9,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,12)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Places",
                type: "numeric(9,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,12)",
                oldNullable: true);
        }
    }
}
