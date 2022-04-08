using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCT.Infrastructure.Migrations
{
    public partial class selectteamteamnameuniqness2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectedTeams_TeamName_PlayerId",
                table: "SelectedTeams");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedTeams_TeamName_TournamentId",
                table: "SelectedTeams",
                columns: new[] { "TeamName", "TournamentId" },
                unique: true,
                filter: "[TournamentId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectedTeams_TeamName_TournamentId",
                table: "SelectedTeams");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedTeams_TeamName_PlayerId",
                table: "SelectedTeams",
                columns: new[] { "TeamName", "PlayerId" },
                unique: true,
                filter: "[PlayerId] IS NOT NULL");
        }
    }
}
