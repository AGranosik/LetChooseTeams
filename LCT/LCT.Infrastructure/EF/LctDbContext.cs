using LCT.Core.Entites.Tournament.Entities;
using Microsoft.EntityFrameworkCore;

namespace LCT.Infrastructure.EF
{
    public class LctDbContext : DbContext, IDbContext
    {
        public LctDbContext()
        {

        }
        public LctDbContext(DbContextOptions<LctDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
         => await base.SaveChangesAsync(cancellationToken);
    }
}
