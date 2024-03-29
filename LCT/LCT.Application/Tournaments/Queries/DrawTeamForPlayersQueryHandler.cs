﻿using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Common.Interfaces;
using MediatR;

namespace LCT.Application.Tournaments.Queries
{
    public class DrawTeamForPlayersQuery : IRequest<List<DrawnTeamDto>>
    {
        public Guid TournamentId { get; set; }
    }

    public record DrawnTeamDto(string name, string surname, string teamName);
    // should be command
    public class DrawTeamForPlayersQueryHandler : IRequestHandler<DrawTeamForPlayersQuery, List<DrawnTeamDto>>
    {
        private readonly ITournamentDomainService _tournamentDomainService;
        private readonly IAggregateRepository<Tournament> _repository;
        public DrawTeamForPlayersQueryHandler(ITournamentDomainService tournamentDomainService, IAggregateRepository<Tournament> repository)
        {
            _tournamentDomainService = tournamentDomainService;
            _repository = repository;
        }
        public async Task<List<DrawnTeamDto>> Handle(DrawTeamForPlayersQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId, cancellationToken);
            if (tournament == null)
                throw new ArgumentNullException();

            tournament.DrawnTeamForPLayers(_tournamentDomainService);
            await _repository.SaveAsync(tournament, cancellationToken: cancellationToken);
            return tournament.DrawTeams.Select(dt => new DrawnTeamDto(dt.Player.Name, dt.Player.Surname, dt.TeamName.Value)).ToList();
        }
    }
}
