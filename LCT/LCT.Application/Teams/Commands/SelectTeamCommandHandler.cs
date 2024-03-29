﻿using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Common.Interfaces;
using MediatR;

namespace LCT.Application.Teams.Commands
{
    public class SelectTeamCommand: IRequest
    {
        public string PlayerName { get; set; }
        public string PlayerSurname { get; set; }
        public Guid TournamentId { get; set; }
        public string Team { get; set; }
    }

    public record SelectTeamMessageDto(Guid PlayerId, string Team);
    public class SelectTeamCommandHandler : IRequestHandler<SelectTeamCommand>
    {
        private readonly IAggregateRepository<Tournament> _repository;
        public SelectTeamCommandHandler(IAggregateRepository<Tournament> repository)
        {
            _repository = repository;
        }
        public async Task Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId, cancellationToken);
            tournament.SelectTeam(request.PlayerName, request.PlayerSurname, request.Team);

            await _repository.SaveAsync(tournament, cancellationToken: cancellationToken);
        }
    }
}
