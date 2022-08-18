using LCT.Core.Shared.BaseTypes;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects
{
    public class DrawnTeam : ValueType<DrawnTeam>
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

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public Tournament Tournament { get; init; }
        public Player Player { get; init; }
        public TeamName TeamName { get; init; }
    }
}
