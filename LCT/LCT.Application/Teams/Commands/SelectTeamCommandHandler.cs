using LCT.Application.Teams.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Infrastructure.Repositories
    ;
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
        private readonly IRepository<Tournament> _repository;
        public SelectTeamCommandHandler(IRepository<Tournament> repository, IMediator mediator)
        {
            _mediator = mediator;
            _repository = repository;
        }
        public async Task<Unit> Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.Load(request.TournamentId);

            if (tournament == null)
                throw new ArgumentException("Turniej nie istnieje.");

            tournament.SelectTeam(request.PlayerId, request.Team);

            await _repository.Save(tournament);

            try
            {
                await _mediator.Publish(new TeamSelectedMessageEvent
                {
                    PlayerId = request.PlayerId,
                    Team = request.Team,
                    TournamentId = request.TournamentId,
                });
            }
            catch (Exception ex)
            {
            }

            return Unit.Value;
        }
    }
}
