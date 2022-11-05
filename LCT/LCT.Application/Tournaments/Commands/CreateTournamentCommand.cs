using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Common.Interfaces;
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
        private readonly IAggregateRepository<Tournament> _repository;
        private readonly IPersistanceClient _dbContext;
        public CreateTournamentCommandHandler(IAggregateRepository<Tournament> repository, IPersistanceClient dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = Tournament.Create(request.Name, request.PlayerLimit);
            await TournamentNameUniqnessValidationAsync(tournament); 
            await _repository.SaveAsync(tournament, tournament.Version);
            return tournament.Id.Value;
        }


        private async Task TournamentNameUniqnessValidationAsync(Tournament tournament)
        {
            var isNameUnique = await _dbContext.CheckUniqness(nameof(Tournament), nameof(Tournament.TournamentName), tournament.TournamentName);
            // check uniqness change name to -> reserve name... and add failure callback
            if (!isNameUnique)
                throw new TournamentNameNotUniqueException();
        }
    }
}
