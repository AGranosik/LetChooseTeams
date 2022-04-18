using LCT.Core.Entites.Tournaments.Entities;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LCT.Application.Tournaments.Queries
{
    public class DrawTeamForPlayersQuery : IRequest<List<DrawnTeam>>
    {
        public Guid TournamentId { get; set; }
    }
    public class DrawTeamForPlayersQueryHandler : IRequestHandler<DrawTeamForPlayersQuery, List<DrawnTeam>>
    {
        private readonly LctDbContext _dbContext;
        public DrawTeamForPlayersQueryHandler(LctDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<DrawnTeam>> Handle(DrawTeamForPlayersQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments
                    .Include(t => t.DrawTeams)
                    .Include(t => t.Players)
                    .Include(t => t.SelectedTeams)
                .FirstOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);

            if (tournament == null)
                throw new ArgumentNullException();

            return tournament.DrawTeams.ToList();
        }
    }
}
