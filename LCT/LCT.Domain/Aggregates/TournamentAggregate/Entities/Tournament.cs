using LCT.Core.Shared.BaseTypes;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Aggregates.TournamentAggregate.Validators;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;

namespace LCT.Domain.Aggregates.TournamentAggregate.Entities
{
    public class Tournament : Aggregate<TournamentId>
    {
        public Tournament() : base(null) { }
        private Tournament(TournamentName tournamentName, TournamentLimit limit) : base(TournamentId.Create())
        {
            Limit = limit;
            TournamentName = tournamentName;
        }
        public TournamentName TournamentName { get; private set; }
        private List<Player> _players = new List<Player>();
        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();
        private List<SelectedTeam> _selectedTeams = new List<SelectedTeam>();
        public virtual IReadOnlyCollection<SelectedTeam> SelectedTeams => _selectedTeams.AsReadOnly();
        private List<DrawnTeam> _drawTeams = new List<DrawnTeam>();
        public virtual IReadOnlyCollection<DrawnTeam> DrawTeams => _drawTeams.AsReadOnly();
        public TournamentLimit Limit { get; private set; }
        public int NumberOfPlayers => _players.Count;

        public void AddPlayer(string name, string surname)
        {
            var playerToValidate = Player.Create(name, surname);
            this.AddPlayerValidation(playerToValidate);
            Apply(new PlayerAdded(playerToValidate.Name, playerToValidate.Surname, Id.Value));
        }
        
        public void SelectTeam(string playerName, string playerSurname, string team)
        {
            var playerToFind = Player.Create(playerName, playerSurname);
            this.SelectTeamValidation(playerToFind, team);
            Apply(new TeamSelected(playerToFind, team, Id.Value));
        }

        public void DrawnTeamForPLayers(ITournamentDomainService service)
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
            {
                var result = service.DrawTeamForPlayers(_selectedTeams);
                foreach (var @event in result)
                    Apply(new DrawTeamEvent(@event.Player, @event.TeamName, Id.Value));
            }
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
                case DrawTeamEvent dt:
                    OnTeamDrawn(dt);
                    break;

            }
        }

        private void OnTeamDrawn(DrawTeamEvent dt)
        {
            _drawTeams.Add(DrawnTeam.Create(dt.Player, dt.TeamName));
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
            Id = TournamentId.Create(tc.StreamId);
        }
    }
}
