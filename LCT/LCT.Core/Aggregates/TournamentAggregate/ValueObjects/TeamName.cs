using LCT.Core.Aggregates.TournamentAggregate.Types;

namespace LCT.Core.Aggregates.TournamentAggregate.ValueObjects
{
    public class TeamName : Name
    {
        public TeamName(string name) : base(name)
        {
        }

        protected override void Validate(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Nazwa druzyny nie moze byc pusta.");

            if (!TournamentTeamNames.TeamExists(name))
                throw new ArgumentNullException("Niepoprawna nazwa druzyny.");
        }


        public static implicit operator string(TeamName name) => name.Value;

        public static implicit operator TeamName(string name) => new(name);
    }
}
