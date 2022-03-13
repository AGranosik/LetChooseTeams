using LCT.Core.Entities.Tournaments.Types;

namespace LCT.Core.Entites.Tournaments.ValueObjects
{
    public class TeamName
    {
        public TeamName(string name)
        {
            Validate(name);
            Name = name;
        }

        public string Name { get; private set; }
        private void Validate(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Nazwa druzyny nie moze byc pusta.");

            if (TournamentTeamNames.TeamExists(name))
                throw new ArgumentNullException("Niepoprawna nazwa druzyny.");
        }
    }
}
