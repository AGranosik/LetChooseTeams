﻿using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using MediatR;

namespace LCT.Application.Players.Events
{
    public class PlayerAddedClientMessage : ClientMessage
    {
        public override string Type => "PlayerAssigned";
        public Guid TournamentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class PlayerAddedEventHandler : INotificationHandler<PlayerAddedDomainEvent>
    {
        private readonly IClientCommunicationService _clientCommuncationService;

        public PlayerAddedEventHandler(IClientCommunicationService clientCommuncationService)
        {
            _clientCommuncationService = clientCommuncationService;
        }

        public async Task Handle(PlayerAddedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _clientCommuncationService.SendAsync(notification.StreamId.ToString(), new PlayerAddedClientMessage
            {
                TournamentId = notification.StreamId,
                Name = notification.Name,
                Surname = notification.Surname
            }, cancellationToken);
        }
    }
}
