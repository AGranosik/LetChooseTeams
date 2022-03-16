using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Entities
{
    public class Tournament : Entity
    {
        private Tournament() { }

        private Tournament(Name tournamentName, TournamentLimit limit)
        {
            Limit = limit;
            TournamentName = tournamentName;
        }
        public Name TournamentName { get; private set; }
        private List<Player> _players = new List<Player>();
        public virtual IReadOnlyCollection<Player> Players => _players;
        private List<SelectedTeam> _selectedTeams = new List<SelectedTeam>();
        public virtual IReadOnlyCollection<SelectedTeam> SelectedTeams => _selectedTeams;
        public TournamentLimit Limit { get; private set; }
        public int NumberOfPlayers => _players.Count;

        public IReadOnlyCollection<Player> AddPlayer(Player player)
        {
            CheckIfPlayerAlreadyExists(player);
            Limit.ChceckIfPlayerCanBeAdded(NumberOfPlayers);
            _players.Add(player);
            return Players;
        }

        private void CheckIfPlayerAlreadyExists(Player player)
        {
            if(_players.Any(p => p == player))
                throw new PlayerAlreadyAssignedToTournamentException();
        }

        public void SelectTeam(SelectedTeam selectedTeam)
        {
            CheckIfPlayerInTournament(selectedTeam.PlayerId);
            CheckIfPlayerNotSelectedTeamBefore(selectedTeam);
            CheckIfTeamAlreadySelected(selectedTeam);
            _selectedTeams.Add(selectedTeam);
        }

        private void CheckIfPlayerInTournament(Guid playerId)
        {
            if (!_players.Select(p => p.Id).Any(id => id == playerId))
                throw new PlayerNotInTournamentException();
        }

        private void CheckIfTeamAlreadySelected(SelectedTeam team)
        {
            if(_selectedTeams.Any(p => p == team))
                throw new TeamAlreadySelectedException();
        }

        private void CheckIfPlayerNotSelectedTeamBefore(SelectedTeam team)
        {
            if (_selectedTeams.Any(p => p.PlayerId == team.PlayerId))
                throw new PlayerSelectedTeamBeforeException();
        }

        public static Tournament Create(Name tournamentName, TournamentLimit limit)
            => new (tournamentName, limit);
    }
}
