using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCT.Infrastructure.Migrations
{
    public partial class playernameuniqnessintournament : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Players_Name_Surname_TournamentId",
                table: "Players",
                columns: new[] { "Name", "Surname", "TournamentId" },
                unique: true,
                filter: "[TournamentId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_Name_Surname_TournamentId",
                table: "Players");
        }
    }
}
