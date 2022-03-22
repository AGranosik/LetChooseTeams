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
        public string Team { get; set; }
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
            var tournament = await _dbContext.Tournaments
                .Include(t => t.Players)
                .Include(t => t.SelectedTeams)
                .SingleOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);

            if (tournament == null)
                throw new ArgumentException("Turniej nie istnieje.");

            tournament.SelectTeam(request.PlayerId, request.Team);

            return Unit.Value;
        }
    }
}
