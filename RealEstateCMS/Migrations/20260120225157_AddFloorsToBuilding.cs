using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFloorsToBuilding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FloorsCount",
                table: "Buildings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FloorsCount",
                table: "Buildings");
        }
    }
}
