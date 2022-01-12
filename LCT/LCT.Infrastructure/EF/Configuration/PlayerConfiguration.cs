using LCT.Core.Entites.Tournament.Entities;
using LCT.Core.Entites.Tournament.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
