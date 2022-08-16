using LCT.Core.Aggregates.TournamentAggregate.Types;
using Microsoft.EntityFrameworkCore;

namespace LCT.Core.Aggregates.TournamentAggregate.ValueObjects
{

    public class TeamName
    {
        public TeamName(string name)
        {
            Validate(name);
            Value = name;
        }

        public string Value { get; private set; }
        private void Validate(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Nazwa druzyny nie moze byc pusta.");

            if (!TournamentTeamNames.TeamExists(name))
                throw new ArgumentNullException("Niepoprawna nazwa druzyny.");
        }


        public static implicit operator string(TeamName name) => name.Value;

        public static implicit operator TeamName(string name) => new(name);


        public static bool operator ==(TeamName a, TeamName b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is not null && b is not null)
            {
                return a.Value.Equals(b.Value);
            }

            return false;
        }

        public static bool operator !=(TeamName a, TeamName b) => !(a == b);

        public override string ToString() => Value;
    }
}
