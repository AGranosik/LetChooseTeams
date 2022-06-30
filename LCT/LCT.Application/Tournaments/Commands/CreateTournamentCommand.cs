using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using LCT.Infrastructure.Repositories;
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
        private readonly IRepository<Tournament> _repository;
        public CreateTournamentCommandHandler(LctDbContext dbContext, IRepository<Tournament> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = Tournament.Create(new Name(request.Name), new TournamentLimit(request.PlayerLimit));
            await _dbContext.Tournaments.AddAsync(tournament, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return tournament.Id;
        }
    }
}
