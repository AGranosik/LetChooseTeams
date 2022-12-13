using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Domain.Common.BaseTypes;
using Newtonsoft.Json;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams
{
    public class SelectedTeam : ValueType<SelectedTeam>
    {
        private SelectedTeam() { }
        private SelectedTeam(Player player, string teamName)
        {
            Player = player;
            TeamName = TeamName.Create(teamName);
        }

        public static SelectedTeam Create(Player player, string teamName)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));
            return new SelectedTeam(player, teamName);
        }
        [JsonProperty]
        public Player Player { get; private set; }
        [JsonProperty]
        public TeamName TeamName { get; private set; }

        public bool IsAlreadyPicked(SelectedTeam team)
        {
            return team.TeamName == TeamName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            SelectedTeam other = obj as SelectedTeam;
            if (other == null) return false;
            return Player == other.Player && TeamName == other.TeamName;
        }
    }
}
