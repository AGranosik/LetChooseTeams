using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LCT.Infrastructure.EF.Configuration
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.Property(x => x.Name).IsRequired()
                .HasMaxLength(80)
                .HasConversion(x => x.Value, x => new Name(x));

            builder.Property(x => x.Surname).IsRequired()
                .HasMaxLength(80)
                .HasConversion(x => x.Value, x => new Name(x));

            //builder.HasKey(new string[] { "Id", "TournamentId" });
            builder.HasIndex(new string[] { nameof(Player.Name), nameof(Player.Surname), "TournamentId" })
                .IsUnique();
        }
    }
}
