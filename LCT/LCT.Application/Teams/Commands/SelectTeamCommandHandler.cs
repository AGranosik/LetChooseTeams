using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entities.Tournaments.Types;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LCT.Application.Teams.Commands
{
    public class SelectTeamCommand: IRequest
    {
        public Guid PlayerId { get; set; }
        public Guid TournamentId { get; set; }
        public List<string> Teams { get; set; }
    }
    public class SelectTeamCommandHandler : IRequestHandler<SelectTeamCommand>
    {
        private readonly LctDbContext _dbContext;
        public SelectTeamCommandHandler(LctDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            if (request.Teams.Count != 2)
                throw new ArgumentException("Nie poprawna liczba wybranych druzyn.");

            foreach(var team in request.Teams)
            {
                if (!TournamentTeamNames.TeamExists(team))
                    throw new ArgumentException("Wybrana druzyna jest niepoprawna.");
            }

            var tournament = await _dbContext.Tournaments
                .Include(t => t.Players)
                .Include(t => t.SelectedTeams)
                .SingleOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);

            if (tournament == null)
                throw new ArgumentException("Turniej nie istnieje.");

            var player = await _dbContext.Players.SingleOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
            if (player is null)
                throw new ArgumentException("Gracz nie istnieje.");

            foreach (var team in request.Teams)
                tournament.SelectTeam(SelectedTeam.Create(player, team));

            return Unit.Value;
        }
    }
}
