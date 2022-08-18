using LCT.Domain.Aggregates.TournamentAggregate.Types;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams
{
    public class TeamName : Name
    {
        public TeamName(string name) : base(name)
        {
        }

        protected override void Validate(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Nazwa druzyny nie moze byc pusta.");

            if (!TournamentTeamNames.TeamExists(name))
                throw new ArgumentNullException("Niepoprawna nazwa druzyny.");
        }
    }
}
