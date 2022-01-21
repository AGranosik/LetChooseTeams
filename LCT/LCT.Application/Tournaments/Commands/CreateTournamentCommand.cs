using LCT.Core.Entites.Tournament.Entities;
using LCT.Core.Entites.Tournament.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;

namespace LCT.Application.Tournaments.Commands
{
    public class CreateTournamentCommand : IRequest
    {
        public string Name { get; set; }
        public int PlayerLimit { get; set; }
    }

    public class CreateTournamentCommandHandler : IRequestHandler<CreateTournamentCommand>
    {
        private readonly IDbContext _dbContext;
        public CreateTournamentCommandHandler(IDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<Unit> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = Tournament.Create(new Name(request.Name), new TournamentLimit(request.PlayerLimit));
            await _dbContext.Tournaments.AddAsync(tournament, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
