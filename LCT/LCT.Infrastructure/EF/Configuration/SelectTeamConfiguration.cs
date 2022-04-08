using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LCT.Infrastructure.EF.Configuration
{
    public class SelectTeamConfiguration : IEntityTypeConfiguration<SelectedTeam>
    {
        public void Configure(EntityTypeBuilder<SelectedTeam> builder)
        {
            builder.Property(x => x.TeamName)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(x => x.Value, x => new TeamName(x));

            builder.HasIndex(new string[] { "TournamentId", "PlayerId" })
                .IsUnique();

            builder.HasIndex(new string[] { "TeamName", "TournamentId" })
                .IsUnique();
        }
    }
}
