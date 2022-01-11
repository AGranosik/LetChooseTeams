using LCT.Core.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
