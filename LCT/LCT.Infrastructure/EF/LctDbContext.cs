﻿using LCT.Core.Entites.Tournaments.Entities;
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
            //modelBuilder.UsePropertyAccessMode(PropertyAccessMode.Field);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

    }
}
