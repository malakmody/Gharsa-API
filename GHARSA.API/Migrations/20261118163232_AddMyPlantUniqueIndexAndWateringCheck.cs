using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GHARSA.API.Migrations
{
    public partial class AddMyPlantUniqueIndexAndWateringCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyPlants_UserId",
                table: "MyPlants");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PlantCareGuides_WateringIntervalDays_Positive",
                table: "PlantCareGuides",
                sql: "[WateringIntervalDays] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_MyPlants_UserId_PlantId",
                table: "MyPlants",
                columns: new[] { "UserId", "PlantId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_PlantCareGuides_WateringIntervalDays_Positive",
                table: "PlantCareGuides");

            migrationBuilder.DropIndex(
                name: "IX_MyPlants_UserId_PlantId",
                table: "MyPlants");

            migrationBuilder.CreateIndex(
                name: "IX_MyPlants_UserId",
                table: "MyPlants",
                column: "UserId");
        }
    }
}
