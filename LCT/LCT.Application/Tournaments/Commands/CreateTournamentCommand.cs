using LCT.Domain.Aggregates.TournamentAggregate.Entities;
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
        public CreateTournamentCommandHandler(IAggregateRepository<Tournament> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            return Guid.NewGuid();
            //var tournament = Tournament.Create(request.Name, request.PlayerLimit);
            //await _repository.SaveAsync(tournament, tournament.Version);
            //return tournament.Id.Value;
        }
    }
}
