﻿using LCT.Core.Entites.Tournament;
using LCT.Core.Entites.Tournament.Entities;
using LCT.Core.Entites.Tournament.ValueObjects;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
