using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Common.Interfaces;
using MediatR;

namespace LCT.Application.Tournaments.Commands
{
    public class SetTournamentNameCommand: IRequest
    {
        public Guid TournamentId { get; set; }
        public string Name { get; set; }
    }

    public class SetTournamentNameCommandHandler : IRequestHandler<SetTournamentNameCommand>
    {
        private readonly IAggregateRepository<Tournament> _repository;
        public SetTournamentNameCommandHandler(IAggregateRepository<Tournament> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(SetTournamentNameCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId);
            tournament.SetName(request.Name);
            await _repository.SaveAsync(tournament, tournament.Version);

            return Unit.Value;
        }
    }
}
