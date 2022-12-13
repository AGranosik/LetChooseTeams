using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Domain.Common.BaseTypes;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams
{
    public class DrawnTeam : ValueType<DrawnTeam>
    {
        DrawnTeam() { }
        private DrawnTeam(Player player, string teamName)
        {
            Player = player;
            TeamName = TeamName.Create(teamName);
        }

        public static DrawnTeam Create(Player player, string teamName)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));
            return new DrawnTeam(player, teamName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            DrawnTeam other = obj as DrawnTeam;
            if (other == null) return false;
            return Player == other.Player && TeamName == other.TeamName;
        }

        public Player Player { get; init; }
        public TeamName TeamName { get; init; }
    }
}
