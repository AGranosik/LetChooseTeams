using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;

namespace LCT.Application.Tournaments.Commands
{
    public class CreateTournamentCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public int PlayerLimit { get; set; }
    }

    public class CreateTournamentCommandHandler : IRequestHandler<CreateTournamentCommand, Guid>
    {
        private readonly LctDbContext _dbContext;
        public CreateTournamentCommandHandler(LctDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<Guid> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = Tournament.Create(new Name(request.Name), new TournamentLimit(request.PlayerLimit));
            await _dbContext.Tournaments.AddAsync(tournament, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return tournament.Id;
        }
    }
}
