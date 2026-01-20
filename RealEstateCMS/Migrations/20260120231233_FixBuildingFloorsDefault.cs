using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCMS.Migrations
{
    /// <inheritdoc />
    public partial class FixBuildingFloorsDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                     UPDATE Buildings
                                     SET FloorsCount = 1
                                     WHERE FloorsCount = 0
                                 """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

    }
}
