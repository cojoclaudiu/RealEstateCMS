using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateCMS.Migrations
{
    /// <inheritdoc />
    public partial class FixImageModelFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Buildings_OwnerId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_HouseTypes_OwnerId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Plots_OwnerId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_OwnerId",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Images",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HouseTypeId",
                table: "Images",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlotId",
                table: "Images",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_BuildingId",
                table: "Images",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_HouseTypeId",
                table: "Images",
                column: "HouseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_PlotId",
                table: "Images",
                column: "PlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Buildings_BuildingId",
                table: "Images",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_HouseTypes_HouseTypeId",
                table: "Images",
                column: "HouseTypeId",
                principalTable: "HouseTypes",
                principalColumn: "HouseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Plots_PlotId",
                table: "Images",
                column: "PlotId",
                principalTable: "Plots",
                principalColumn: "PlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Buildings_BuildingId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_HouseTypes_HouseTypeId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Plots_PlotId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_BuildingId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_HouseTypeId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_PlotId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "HouseTypeId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PlotId",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_Images_OwnerId",
                table: "Images",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Buildings_OwnerId",
                table: "Images",
                column: "OwnerId",
                principalTable: "Buildings",
                principalColumn: "BuildingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_HouseTypes_OwnerId",
                table: "Images",
                column: "OwnerId",
                principalTable: "HouseTypes",
                principalColumn: "HouseTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Plots_OwnerId",
                table: "Images",
                column: "OwnerId",
                principalTable: "Plots",
                principalColumn: "PlotId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
