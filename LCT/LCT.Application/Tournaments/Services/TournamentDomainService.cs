using LCT.Application.Common.Interfaces;
using LCT.Core.Extensions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;

namespace LCT.Core.Entites.Tournaments.Services
{
    public class TournamentDomainService : ITournamentDomainService
    {
        private readonly IPersistanceClient _dbContext;
        public TournamentDomainService(IPersistanceClient dbContext)
        {
            _dbContext = dbContext;
        }
        public List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams)
        {
            var playerList = selectedTeams
                .Select(st => st.Player)
                .Shuffle();

            var teams = selectedTeams
                .Select(st => st.TeamName)
                .Shuffle()
                .ToList();

            var result = playerList.Select((p, index) => DrawnTeam.Create(p, teams[index])).ToList();

            return result;
        }

    }
}
