using LCT.Core.Aggregates.TournamentAggregate.Services;
using LCT.Core.Entites.Tournaments.Services;
using MediatR;

namespace LCT.Application.Tournaments.Queries
{
    public class DrawTeamForPlayersQuery : IRequest<List<DrawnTeamDto>>
    {
        public Guid TournamentId { get; set; }
    }

    public record DrawnTeamDto(Guid playerId, string teamName);
    public class DrawTeamForPlayersQueryHandler : IRequestHandler<DrawTeamForPlayersQuery, List<DrawnTeamDto>>
    {
        private readonly ITournamentDomainService _tournamentDomainService;
        public DrawTeamForPlayersQueryHandler(ITournamentDomainService tournamentDomainService)
        {
            _tournamentDomainService = tournamentDomainService;
        }
        public async Task<List<DrawnTeamDto>> Handle(DrawTeamForPlayersQuery request, CancellationToken cancellationToken)
        {
            return null;
            //var tournament = await _dbContext.Tournaments
            //        .Include(t => t.DrawTeams)
            //        .Include(t => t.Players)
            //        .Include(t => t.SelectedTeams)
            //    .FirstOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);

            //if (tournament == null)
            //    throw new ArgumentNullException();

            //tournament.DrawnTeamForPLayers(_tournamentDomainService);

            //await _dbContext.SaveChangesAsync();
            //return tournament.DrawTeams.Select(dt => new DrawnTeamDto(dt.Player.Id, dt.TeamName.Value)).ToList();
        }
    }
}
