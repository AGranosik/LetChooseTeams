using LCT.Core.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Core.Shared;

namespace LCT.Core.Aggregates.TournamentAggregate.Entities
{
    public class DrawnTeam : Entity
    {
        private DrawnTeam() { }
        private DrawnTeam(Player player, string teamName)
        {
            Player = player;
            TeamName = new TeamName(teamName);
        }

        public static DrawnTeam Create(Player player, string teamName)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));
            return new DrawnTeam(player, teamName);
        }

        public Tournament Tournament { get; private set; }
        public Player Player { get; private set; }
        public TeamName TeamName { get; private set; }
    }
}
