using LCT.Application.Teams.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Infrastructure.Repositories
    ;
using MediatR;

namespace LCT.Application.Teams.Commands
{
    public class SelectTeamCommand: IRequest
    {
        public string PlayerName { get; set; }
        public string PlayerSurname { get; set; }
        public Guid TournamentId { get; set; }
        public string Team { get; set; }
    }

    public record SelectTeamMessageDto(Guid playerId, string team);
    public class SelectTeamCommandHandler : IRequestHandler<SelectTeamCommand>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Tournament> _repository;
        private readonly ITournamentDomainService _tournamentDomainService;
        //remove mediator
        public SelectTeamCommandHandler(IRepository<Tournament> repository, IMediator mediator, ITournamentDomainService tournamentDomainService)
        {
            _mediator = mediator;
            _repository = repository;
            _tournamentDomainService = tournamentDomainService;
        }
        public async Task<Unit> Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId);

            if (tournament == null)
                throw new ArgumentException("Turniej nie istnieje.");

            tournament.SelectTeam(request.PlayerName, request.PlayerSurname, request.Team);
            await _tournamentDomainService.PlayerTeamSelectionValidationAsync(request.Team, request.TournamentId);
            await _repository.SaveAsync(tournament);

            try
            {
                await _mediator.Publish(new TeamSelectedMessageEvent
                {
                    PlayerSurname = request.PlayerSurname,
                    PlayerName = request.PlayerName,
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
