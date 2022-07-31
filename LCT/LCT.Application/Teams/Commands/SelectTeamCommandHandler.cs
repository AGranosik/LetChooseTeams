using MediatR;

namespace LCT.Application.Teams.Commands
{
    public class SelectTeamCommand: IRequest
    {
        public Guid PlayerId { get; set; }
        public Guid TournamentId { get; set; }
        public string Team { get; set; }
    }

    public record SelectTeamMessageDto(Guid playerId, string team);
    public class SelectTeamCommandHandler : IRequestHandler<SelectTeamCommand>
    {
        private readonly IMediator _mediator;
        public SelectTeamCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<Unit> Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            //var tournament = await _dbContext.Tournaments
            //    .Include(t => t.Players)
            //    .Include(t => t.SelectedTeams)
            //    .SingleOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);

            //if (tournament == null)
            //    throw new ArgumentException("Turniej nie istnieje.");

            //tournament.SelectTeam(request.PlayerId, request.Team);

            //await _dbContext.SaveChangesAsync(cancellationToken);

            //try
            //{
            //    await _mediator.Publish(new TeamSelectedMessageEvent
            //    {
            //        PlayerId = request.PlayerId,
            //        Team = request.Team,
            //        TournamentId = request.TournamentId,
            //    });
            //}
            //catch (Exception ex)
            //{
            //}

            return Unit.Value;
        }
    }
}
