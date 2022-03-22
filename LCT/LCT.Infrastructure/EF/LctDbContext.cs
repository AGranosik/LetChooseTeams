using LCT.Core.Entites;
using LCT.Core.Entites.Tournaments.Entities;
using Microsoft.EntityFrameworkCore;

namespace LCT.Infrastructure.EF
{
    public class LctDbContext : DbContext
    {
        public LctDbContext()
        {

        }
        public LctDbContext(DbContextOptions<LctDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        public virtual DbSet<SelectedTeam> SelectedTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Entity && (
                        e.State == EntityState.Added));

            foreach(var entry in entries)
            {
                ((Entity)entry.Entity).CreatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
