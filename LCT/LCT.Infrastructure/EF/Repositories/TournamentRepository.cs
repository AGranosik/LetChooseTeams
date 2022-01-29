using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LCT.Infrastructure.EF.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly LctDbContext _context;
        public TournamentRepository(LctDbContext context)
        {
            _context = context;
        }
        public Task<Tournament> GetTournament(Guid Id)
            => _context.Tournaments.SingleOrDefaultAsync(t => t.Id == Id);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _context.SaveChangesAsync(cancellationToken);
    }
}
