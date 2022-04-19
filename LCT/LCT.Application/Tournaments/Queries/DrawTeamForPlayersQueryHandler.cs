﻿using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Services;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LCT.Application.Tournaments.Queries
{
    public class DrawTeamForPlayersQuery : IRequest<List<DrawnTeam>>
    {
        public Guid TournamentId { get; set; }
    }
    public class DrawTeamForPlayersQueryHandler : IRequestHandler<DrawTeamForPlayersQuery, List<DrawnTeam>>
    {
        private readonly LctDbContext _dbContext;
        private readonly ITournamentDomainService _tournamentDomainService;
        public DrawTeamForPlayersQueryHandler(LctDbContext dbContext, ITournamentDomainService tournamentDomainService)
        {
            _dbContext = dbContext;
            _tournamentDomainService = tournamentDomainService;
        }
        public async Task<List<DrawnTeam>> Handle(DrawTeamForPlayersQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments
                    .Include(t => t.DrawTeams)
                    .Include(t => t.Players)
                    .Include(t => t.SelectedTeams)
                .FirstOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);

            if (tournament == null)
                throw new ArgumentNullException();

            tournament.DrawnTeamForPLayers(_tournamentDomainService);

            await _dbContext.SaveChangesAsync();
            return tournament.DrawTeams.ToList();
        }
    }
}
