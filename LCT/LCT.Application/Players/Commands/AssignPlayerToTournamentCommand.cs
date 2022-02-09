﻿using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Repositories;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LCT.Application.Players.Commands
{
    public class AssignPlayerToTournamentCommand: IRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid TournamentId { get; set; }
    }


    public class AssignPlayerToTournamentCommandHandler : IRequestHandler<AssignPlayerToTournamentCommand>
    {
        private readonly LctDbContext _dbContext;
        public AssignPlayerToTournamentCommandHandler(LctDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == request.TournamentId);
            var player = Player.Register(new Name(request.Name), new Name(request.Surname));

            tournament.AddPlayer(player);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

}
