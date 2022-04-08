using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCT.Infrastructure.Migrations
{
    public partial class selectteamuniqness2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectedTeams_TournamentId_PlayerId_TeamName",
                table: "SelectedTeams");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedTeams_TournamentId_PlayerId",
                table: "SelectedTeams",
                columns: new[] { "TournamentId", "PlayerId" },
                unique: true,
                filter: "[TournamentId] IS NOT NULL AND [PlayerId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectedTeams_TournamentId_PlayerId",
                table: "SelectedTeams");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedTeams_TournamentId_PlayerId_TeamName",
                table: "SelectedTeams",
                columns: new[] { "TournamentId", "PlayerId", "TeamName" },
                unique: true,
                filter: "[TournamentId] IS NOT NULL AND [PlayerId] IS NOT NULL");
        }
    }
}
