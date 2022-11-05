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
        private readonly IPersistanceClient _dbContext;
        public SelectTeamCommandHandler(IAggregateRepository<Tournament> repository, IPersistanceClient dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId);
            tournament.SelectTeam(request.PlayerName, request.PlayerSurname, request.Team);

            await PlayerTeamSelectionValidationAsync(request.Team, tournament.Id.Value);
            await _repository.SaveAsync(tournament);

            return Unit.Value;
        }

        private async Task PlayerTeamSelectionValidationAsync(string team, Guid tournamentId)
        {
            var isNameUnique = await _dbContext.CheckUniqness(nameof(Tournament), nameof(Tournament.SelectedTeams), new TeamSelectionUniqnessModel(team, tournamentId));

            if (!isNameUnique)
                throw new PlayerAlreadyAssignedToTournamentException();
        }
    }
}
