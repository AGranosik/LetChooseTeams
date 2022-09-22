﻿using LCT.Application.Common.Interfaces;
using LCT.Application.Common.UniqnessModels;
using LCT.Core.Extensions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;

namespace LCT.Core.Entites.Tournaments.Services
{
    public class TournamentDomainService : ITournamentDomainService
    {
        private readonly IPersistanceClient _dbContext;
        public TournamentDomainService(IPersistanceClient dbContext)
        {
            _dbContext = dbContext;
        }
        public List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams)
        {
            var playerList = selectedTeams
                .Select(st => st.Player)
                .Shuffle();

            var teams = selectedTeams
                .Select(st => st.TeamName)
                .Shuffle()
                .ToList();

            var result = playerList.Select((p, index) => DrawnTeam.Create(p, teams[index])).ToList();

            if (result.Any(r => selectedTeams.Any(st => st.Player == r.Player && st.TeamName == r.TeamName)))
                return DrawTeamForPlayers(selectedTeams);

            return result;
        }

        public async Task PlayerTeamSelectionValidationAsync(string team,Guid tournamentId)
        {
            var isNameUnique = await _dbContext.CheckUniqness(nameof(Tournament), nameof(Tournament.SelectedTeams), new TeamSelectionUniqnessModel(team, tournamentId));

            if (!isNameUnique)
                throw new PlayerAlreadyAssignedToTournamentException();
        }

        public async Task TournamentNameUniqnessValidationAsync(Tournament tournament)
        {
            var isNameUnique = await _dbContext.CheckUniqness(nameof(Tournament), nameof(Tournament.TournamentName), tournament.TournamentName);
            if (!isNameUnique)
                throw new TournamentNameNotUniqueException();
        }
    }
}
