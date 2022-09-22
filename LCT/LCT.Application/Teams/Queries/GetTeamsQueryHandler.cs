using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Common.Interfaces;
using MediatR;

namespace LCT.Application.Teams.Queries
{
    public record TeamToSelectDto(string Name, bool Selected);
    public class GetTeamsQuery : IRequest<List<TeamToSelectDto>>
    {
        public Guid TournamentId { get; set; }
    }
    public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, List<TeamToSelectDto>>
    {
        private readonly IAggregateRepository<Tournament> _repository;
        public GetTeamsQueryHandler(IAggregateRepository<Tournament> repository)
        {
            _repository = repository;
        }
        public async Task<List<TeamToSelectDto>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId);
            var alreadySelected = tournament.SelectedTeams;

            return TournamentTeamNames.Teams.Select(t => new TeamToSelectDto(t, alreadySelected.Any(selected => selected.TeamName == t))).ToList();
        }
        
    }
}
