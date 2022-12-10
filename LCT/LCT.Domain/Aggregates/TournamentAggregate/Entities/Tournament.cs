using System.Text.Json.Serialization;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Aggregates.TournamentAggregate.Validators;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;

namespace LCT.Domain.Aggregates.TournamentAggregate.Entities
{
    public class Tournament : Aggregate<TournamentId>, IVersionable<int>
    {
        public Tournament() : base(null) { }
        private Tournament(TournamentName tournamentName, TournamentLimit limit) : base(TournamentId.Create())
        {
            Limit = limit;
            TournamentName = tournamentName;
        }
        [JsonInclude]
        public TournamentName TournamentName { get; private set; }
        private List<Player> _players = new();
        
        [JsonInclude]
        public List<Player> Players => _players;
        private List<SelectedTeam> _selectedTeams = new();

        [JsonInclude]
        public IReadOnlyCollection<SelectedTeam> SelectedTeams => _selectedTeams.AsReadOnly();
        private List<DrawnTeam> _drawTeams = new();

        [JsonInclude]
        public IReadOnlyCollection<DrawnTeam> DrawTeams => _drawTeams.AsReadOnly();

        [JsonInclude]
        public TournamentLimit Limit { get; private set; }

        [JsonInclude]
        public int NumberOfPlayers => _players.Count;

        private int _version = 0;
        [JsonInclude]
        public int Version => _version;

        public void SetName(string tournamentName)
        {
            if (tournamentName == TournamentName)
                throw new InvalidFieldException(nameof(TournamentName));
            Apply(new SetTournamentNameEvent(tournamentName, Id.Value));
        }

        public void AddPlayer(string name, string surname)
        {
            var playerToValidate = Player.Create(name, surname);
            this.AddPlayerValidation(playerToValidate);
            Apply(new PlayerAddedDomainEvent(playerToValidate.Name, playerToValidate.Surname, Id.Value));
        }
        
        public void SelectTeam(string playerName, string playerSurname, string team)
        {
            var playerToFind = Player.Create(playerName, playerSurname);
            this.SelectTeamValidation(playerToFind, team);
            Apply(new TeamSelectedDomainEvent(playerToFind, team, Id.Value));
        }

        public void DrawnTeamForPLayers(ITournamentDomainService service)
        {
            if(service is null)
                throw new ArgumentNullException(nameof(service));

            if (SelectedTeams is null)
                throw new ArgumentNullException("Any team selected.");

            if (NumberOfPlayers != Limit.Limit)
                throw new NotAllPlayersRegisteredException();

            if (SelectedTeams.Count == 0 || SelectedTeams.Count != Limit.Limit)
                throw new NotAllPlayersSelectedTeamException();

            if (_drawTeams is null || _drawTeams.Count == 0)
            {
                var result = service.DrawTeamForPlayers(_selectedTeams);
                foreach (var @event in result)
                    Apply(new DrawTeamEvent(@event.Player, @event.TeamName, Id.Value));
            }
        }

        public static Tournament Create(string tournamentName, int limit)
        {
            var t = new Tournament();
            var streamId = Guid.NewGuid();
            t.Apply(new TournamentCreatedDomainEvent(tournamentName, limit, streamId));
            t.Apply(new SetTournamentNameEvent(tournamentName, streamId));
            return t;
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case TournamentCreatedDomainEvent tc:
                    OnCreated(tc);
                    break;
                case PlayerAddedDomainEvent pa:
                    OnPlayerAdded(pa);
                    break;
                case TeamSelectedDomainEvent ts:
                    OnTeamSelect(ts);
                    break;
                case DrawTeamEvent dt:
                    OnTeamDrawn(dt);
                    break;
                case SetTournamentNameEvent tn:
                    OnSetTournamentName(tn);
                    break;
            }
        }

        private void OnSetTournamentName(SetTournamentNameEvent tn)
        {
            TournamentName = new TournamentName(tn.TournamentName);
            Incerement();
        }

        private void OnTeamDrawn(DrawTeamEvent dt)
        {
            _drawTeams.Add(DrawnTeam.Create(dt.Player, dt.TeamName));
        }

        private void OnTeamSelect(TeamSelectedDomainEvent ts)
        {
            var player = _players.FirstOrDefault(p => p == ts.Player);
            _selectedTeams.Add(SelectedTeam.Create(player, ts.TeamName));
        }

        private void OnPlayerAdded(PlayerAddedDomainEvent pa)
        {
            _players.Add(Player.Create(pa.Name, pa.Surname));
        }

        private void OnCreated(TournamentCreatedDomainEvent tc)
        {
            Limit = tc.Limit;
            Id = TournamentId.Create(tc.StreamId);
        }

        public void Incerement()
            => _version++;
    }
}
