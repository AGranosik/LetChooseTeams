﻿using LCT.Core.Entites.Tournaments.Events;
using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Entites.Tournaments.Services;
using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Entities
{
    public class Tournament : Aggregate
    {
        public Tournament() { }

        private Tournament(Name tournamentName, TournamentLimit limit)
        {
            Limit = limit;
            TournamentName = tournamentName;
        }
        private Guid Id { get; set; }
        public Name TournamentName { get; private set; }
        private List<Player> _players = new List<Player>();
        public IReadOnlyCollection<Player> Players => _players;
        private List<SelectedTeam> _selectedTeams = new List<SelectedTeam>();
        public virtual IReadOnlyCollection<SelectedTeam> SelectedTeams => _selectedTeams;
        private List<DrawnTeam> _drawTeams = new List<DrawnTeam>();
        public virtual IReadOnlyCollection<DrawnTeam> DrawTeams => _drawTeams;
        public TournamentLimit Limit { get; private set; }
        public int NumberOfPlayers => _players.Count;

        public void AddPlayer(Player player)
        {
            CheckIfPlayerAlreadyExists(player);
            Limit.ChceckIfPlayerCanBeAdded(NumberOfPlayers);
            Apply(new PlayerAdded(player.Name, player.Surname, Id));
        }

        private void CheckIfPlayerAlreadyExists(Player player)
        {
            if(_players.Any(p => p == player))
                throw new PlayerAlreadyAssignedToTournamentException();
        }

        public void SelectTeam(Guid playerId, string team)
        {
            var player = _players.FirstOrDefault(p => p.Id == playerId);
            CheckIfPlayerInTournament(player);
            var selectedTeam = SelectedTeam.Create(player, team);
            CheckIfPlayerNotSelectedTeamBefore(selectedTeam);
            CheckIfTeamAlreadySelected(selectedTeam);
            _selectedTeams.Add(selectedTeam);
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
            }
        }

        private void OnPlayerAdded(PlayerAdded pa)
        {
            _players.Add(Player.Register(pa.Name, pa.Surname));
        }

        private void OnCreated(TournamentCreated tc)
        {
            TournamentName = tc.Name;
            Limit = tc.Limit;
            Id = tc.StreamId;
        }
    }
}
