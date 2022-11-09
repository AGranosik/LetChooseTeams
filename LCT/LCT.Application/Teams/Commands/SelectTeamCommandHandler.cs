using LCT.Application.Common.Interfaces;
using LCT.Application.Common.UniqnessModels;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Common.Interfaces;
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
        private readonly IAggregateRepository<Tournament> _repository;
        public SelectTeamCommandHandler(IAggregateRepository<Tournament> repository)
        {
            _repository = repository;
        }
        public async Task<Unit> Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId);
            tournament.SelectTeam(request.PlayerName, request.PlayerSurname, request.Team);

            await _repository.SaveAsync(tournament);

            return Unit.Value;
        }
    }
}
