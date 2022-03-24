using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LCT.Infrastructure.EF.Configuration
{
    public class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
    {
        public void Configure(EntityTypeBuilder<Tournament> builder)
        {
            builder.Property(x => x.Limit).IsRequired()
                .HasConversion(x => x.Limit, x => new TournamentLimit(x));
            builder.HasIndex(t => t.TournamentName)
                .IsUnique();

            builder.Property(x => x.TournamentName)
                .IsRequired()
                .HasMaxLength(80)
                .HasConversion(x => x.Value, x => new Name(x));

        }
    }
}
