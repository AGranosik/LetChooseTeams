using LCT.Core.Entites.Tournament;
using LCT.Core.Entites.Tournament.Entities;
using Microsoft.EntityFrameworkCore;

namespace LCT.Infrastructure.EF
{
    public class LctDbContext : DbContext
    {
        public LctDbContext(DbContextOptions<LctDbContext> options) : base(options)
        {

        }
        public DbSet<Player> Players { get; set; }
    }
}
