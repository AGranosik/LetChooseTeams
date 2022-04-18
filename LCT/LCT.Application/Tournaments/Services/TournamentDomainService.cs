using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Extensions;

namespace LCT.Core.Entites.Tournaments.Services
{
    public class TournamentDomainService : ITournamentDomainService
    {
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

            if (result.Any(r => selectedTeams.Any(st => st.Player == r.Player && st.TeamName == r.TeamName)))
                return DrawTeamForPlayers(selectedTeams);

            return result;
        }
    }
}
