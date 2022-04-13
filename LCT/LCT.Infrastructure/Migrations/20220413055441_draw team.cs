using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCT.Infrastructure.Migrations
{
    public partial class drawteam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrawnTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawnTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawnTeams_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DrawnTeams_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrawnTeams_PlayerId",
                table: "DrawnTeams",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawnTeams_TeamName_TournamentId",
                table: "DrawnTeams",
                columns: new[] { "TeamName", "TournamentId" },
                unique: true,
                filter: "[TournamentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DrawnTeams_TournamentId_PlayerId",
                table: "DrawnTeams",
                columns: new[] { "TournamentId", "PlayerId" },
                unique: true,
                filter: "[TournamentId] IS NOT NULL AND [PlayerId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawnTeams");
        }
    }
}
