using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;

namespace LCT.Domain.Aggregates.TournamentAggregate.Validators
{
    internal static class TournamentValidator
    {
        public static void AddPlayerValidation(this Tournament tournament, Player player)
        {
            CheckPlayerAlreadyInTournament(tournament, player);
            tournament.Limit.ChceckIfPlayerCanBeAdded(tournament.NumberOfPlayers);
        }

        public static void SelectTeamValidation(this Tournament tournament, Player player, string team)
        {
            CheckIfPlayerInTournament(tournament, player);
            var selectedTeam = SelectedTeam.Create(player, team);
            CheckIfPlayerNotSelectedTeamBefore(tournament, selectedTeam);
            CheckIfTeamAlreadySelected(tournament, selectedTeam);
        }

        private static void CheckIfTeamAlreadySelected(Tournament tournament, SelectedTeam team)
        {
            if (tournament.SelectedTeams.Any(p => p.IsAlreadyPicked(team)))
                throw new TeamAlreadySelectedException();
        }

        private static void CheckIfPlayerNotSelectedTeamBefore(Tournament tournament, SelectedTeam team)
        {
            if (tournament.SelectedTeams.Any(p => p.Player == team.Player))
                throw new PlayerSelectedTeamBeforeException();
        }

        private static void CheckIfPlayerInTournament(Tournament tournament, Player player)
        {
            var hehe = tournament.Players.Any(p => p == player);
            if (!tournament.Players.Any(p => p == player))
                throw new PlayerNotInTournamentException();
        }

        private static void CheckPlayerAlreadyInTournament(Tournament tournament, Player player)
        {
            if (IsPlayerExists(tournament, player))
                throw new PlayerAlreadyAssignedToTournamentException();
        }

        private static bool IsPlayerExists(Tournament tournament, Player player)
            => tournament.Players.Any(p => p == player);
    }
}
