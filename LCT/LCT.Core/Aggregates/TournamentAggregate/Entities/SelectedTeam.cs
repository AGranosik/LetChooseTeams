using LCT.Core.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Core.Shared;

namespace LCT.Core.Aggregates.TournamentAggregate.Entities
{
    public class SelectedTeam : Entity
    {
        private SelectedTeam() { }
        private SelectedTeam(Player player, string teamName)
        {
            Player = player;
            TeamName = new TeamName(teamName);
        }

        public static SelectedTeam Create(Player player, string teamName)
        {
            if(player is null)
                throw new ArgumentNullException(nameof(player));
            return new SelectedTeam(player, teamName);
        } 

        public Tournament Tournament { get; private set; }
        public Player Player { get; private set; }
        public TeamName TeamName { get; private set; }

        public bool IsAlreadyPicked(SelectedTeam team)
        {
            return team.TeamName == TeamName;
        }
    }
}
