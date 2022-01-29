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
        private List<Player> _players = new List<Player>();
        public Name TournamentName { get; private set; }
        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();
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

        public static Tournament Create(Name tournamentName, TournamentLimit limit)
            => new (tournamentName, limit);
    }
}
