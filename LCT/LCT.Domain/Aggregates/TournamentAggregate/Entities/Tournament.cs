using LCT.Core.Shared.BaseTypes;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.Entities
{
    public class Tournament : Aggregate
    {
        public Tournament() { }

        private Tournament(TournamentName tournamentName, TournamentLimit limit)
        {
            Limit = limit;
            TournamentName = tournamentName;
        }
        public Guid Id { get; private set; }
        public TournamentName TournamentName { get; private set; }
        private List<Player> _players = new List<Player>();
        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();
        private List<SelectedTeam> _selectedTeams = new List<SelectedTeam>();
        public virtual IReadOnlyCollection<SelectedTeam> SelectedTeams => _selectedTeams.AsReadOnly();
        private List<DrawnTeam> _drawTeams = new List<DrawnTeam>();
        public virtual IReadOnlyCollection<DrawnTeam> DrawTeams => _drawTeams.AsReadOnly();
        public TournamentLimit Limit { get; private set; }
        public int NumberOfPlayers => _players.Count;

        public Guid AddPlayer(string name, string surname)
        {
            var playerId = Guid.NewGuid();
            var player = Player.Create(name, surname);
            CheckIfPlayerAlreadyExists(player);
            Limit.ChceckIfPlayerCanBeAdded(NumberOfPlayers);
            Apply(new PlayerAdded(player.Name, player.Surname, playerId, Id));

            return playerId;
        }
        
        private void CheckIfPlayerAlreadyExists(Player player)
        {
            if(_players.Any(p => p == player))
                throw new PlayerAlreadyAssignedToTournamentException();
        }

        public void SelectTeam(string playerName, string playerSurname, string team)
        {
            var playerToFind = Player.Create(playerName, playerSurname);
            var player = _players.FirstOrDefault(p => p == playerToFind);
            CheckIfPlayerInTournament(player);
            var selectedTeam = SelectedTeam.Create(player, team);
            CheckIfPlayerNotSelectedTeamBefore(selectedTeam);
            CheckIfTeamAlreadySelected(selectedTeam);
            Apply(new TeamSelected(player, team, Id));
        }

        public void DrawnTeamForPLayers(ITournamentDomainService service) // to chyba do wywalenia
        {
            if(service is null)
                throw new ArgumentNullException(nameof(service));

            if (SelectedTeams is null)
                throw new ArgumentNullException("Any team selected.");

            if (NumberOfPlayers != Limit.Limit)
                throw new NotAllPlayersRegisteredException();

            if (SelectedTeams.Count() == 0 || SelectedTeams.Count() != Limit.Limit)
                throw new NotAllPlayersSelectedTeamException();

            if (_drawTeams is null || _drawTeams.Count() == 0)
                _drawTeams = service.DrawTeamForPlayers(_selectedTeams);
        }

        private void CheckIfPlayerInTournament(Player player)
        {
            if (!_players.Any(p => p == player))
                throw new PlayerNotInTournamentException();
        }

        private void CheckIfTeamAlreadySelected(SelectedTeam team)
        {
            if(_selectedTeams.Any(p => p.IsAlreadyPicked(team)))
                throw new TeamAlreadySelectedException();
        }

        private void CheckIfPlayerNotSelectedTeamBefore(SelectedTeam team)
        {
            if (_selectedTeams.Any(p => p.Player == team.Player))
                throw new PlayerSelectedTeamBeforeException();
        }

        public static Tournament Create(string tournamentName, int limit)
        {
            var t = new Tournament();
            t.Apply(new TournamentCreated(tournamentName, limit, Guid.NewGuid()));
            return t;
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case TournamentCreated tc:
                    OnCreated(tc);
                    break;
                case PlayerAdded pa:
                    OnPlayerAdded(pa);
                    break;
                case TeamSelected ts:
                    OnTeamSelect(ts);
                    break;
            }
        }

        private void OnTeamSelect(TeamSelected ts)
        {
            var player = _players.FirstOrDefault(p => p == ts.Player);
            _selectedTeams.Add(SelectedTeam.Create(player, ts.TeamName));
        }

        private void OnPlayerAdded(PlayerAdded pa)
        {
            _players.Add(Player.Create(pa.Name, pa.Surname));
        }

        private void OnCreated(TournamentCreated tc)
        {
            TournamentName = new TournamentName(tc.Name);
            Limit = tc.Limit;
            Id = tc.StreamId;
            // jak dodawaćte dodatkowe dane typu Id i timestamp
        }
    }
}
