using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCT.Infrastructure.Migrations
{
    public partial class selectteamteamnameuniqness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SelectedTeams_TeamName_PlayerId",
                table: "SelectedTeams",
                columns: new[] { "TeamName", "PlayerId" },
                unique: true,
                filter: "[PlayerId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectedTeams_TeamName_PlayerId",
                table: "SelectedTeams");
        }
    }
}
