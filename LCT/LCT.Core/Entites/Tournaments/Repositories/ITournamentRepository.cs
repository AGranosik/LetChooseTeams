using LCT.Core.Entites.Tournaments.Entities;

namespace LCT.Core.Entites.Tournaments.Repositories
{
    public interface ITournamentRepository
    {
        Task<Tournament> GetTournament(Guid Id);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
