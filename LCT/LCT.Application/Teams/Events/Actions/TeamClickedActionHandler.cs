﻿using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Common.Interfaces;
using MediatR;
using Serilog;

namespace LCT.Application.Teams.Events.Actions
{
    public class TeamClickedEvent : ClientMessage
    {
        public override string Type => "TeamClicked";
        public Guid TournamentId { get; set; }
        public List<ClieckedPlayerTeam> ClickedTeams { get; set; }
    }

    public class ClieckedPlayerTeam
    {
        public string Team { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
    public class TeamClickedAction : LctAction<Guid>
    {
        public string Team { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
    public class TeamClickedActionHandler : INotificationHandler<TeamClickedAction>
    {
        private readonly ILctActionRepository<TeamClickedAction, Guid> _repository;
        private readonly IClientCommunicationService _clientCommunicationService;
        public TeamClickedActionHandler(ILctActionRepository<TeamClickedAction, Guid> repository, IClientCommunicationService clientCommunicationService)
        {
            _repository = repository;
            _clientCommunicationService = clientCommunicationService;
        }
        public async Task Handle(TeamClickedAction notification, CancellationToken cancellationToken)
        {
            var saved = await SaveAsync(notification);
            if (!saved)
                return;
            var allClickedTeams = await GetLatesClickedTeams(notification);
            await _clientCommunicationService.SendAsync(notification.GroupKey.ToString(), allClickedTeams, cancellationToken);
        }

        private async Task<TeamClickedEvent> GetLatesClickedTeams(TeamClickedAction action)
        {
            var actions = await _repository.GetByGroupIdAsync(action.GroupKey);
            var grouppedByPlayer = actions
                .GroupBy(a => new { a.Surname, a.Name })
                .Select(a => new ClieckedPlayerTeam
                {
                    Name = a.Key.Name,
                    Surname = a.Key.Surname,
                    Team = a.OrderByDescending(t => t.SavedTime).First().Team
                })
                .ToList();

            return new TeamClickedEvent
            {
                ClickedTeams = grouppedByPlayer,
                TournamentId = action.GroupKey
            };
        }

        private async Task<bool> SaveAsync(TeamClickedAction action)
        {
            if (!IsTeamExist(action.Team))
            {
                Log.Error("Clicked {Team} does not exist.", action.Team);
                return false;
            }
            await _repository.SaveAsync(action);
            return true;
        }

        private bool IsTeamExist(string teamName)
            => TournamentTeamNames.Teams.Any(t => teamName == t);

    }
}
