using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Shared.Exceptions;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LCT.Application.Tournaments.Queries
{
    public class TournamentDto
    {
        public string TournamentName { get; set; }
        public List<PlayerDto> Players { get; set; }

    }

    public class PlayerDto
    {
        public string Name { get; set; }
    }
    public class GetTournamentQuery : IRequest<TournamentDto>
    {
        public Guid TournamentId { get; set; }
    }
    public class GetTournamentQueryHandler : IRequestHandler<GetTournamentQuery, TournamentDto>
    {
        private readonly LctDbContext _dbContext;
        public GetTournamentQueryHandler(LctDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TournamentDto> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments
                .Where(t => t.Id == request.TournamentId)
                .Select(t => new TournamentDto
                    {
                        TournamentName = t.TournamentName.ToString(),
                        Players = t.Players.Select(p => new PlayerDto
                        {
                            Name = p.Name
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tournament == null)
                throw new EntityDoesNotExist(nameof(Tournament));

            return tournament;
        }
    }
}
