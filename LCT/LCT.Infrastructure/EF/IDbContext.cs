using LCT.Core.Entites.Tournament.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Infrastructure.EF
{
    public interface IDbContext
    {
        DbSet<Player> Players { get; set; }
        DbSet<Tournament> Tournaments { get; set; }
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
