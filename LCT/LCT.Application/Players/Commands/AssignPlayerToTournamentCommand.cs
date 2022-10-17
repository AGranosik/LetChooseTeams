using LCT.Application.Players.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Common.Interfaces;
using MediatR;
using Serilog;

namespace LCT.Application.Players.Commands
{
    public class AssignPlayerToTournamentCommand: IRequest<Unit>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid TournamentId { get; set; }
    }

    public record PlayerAssignedMessageDto(Guid Id, string Name, string Surname);
    public class AssignPlayerToTournamentCommandHandler : IRequestHandler<AssignPlayerToTournamentCommand, Unit>
    {
        private readonly IAggregateRepository<Tournament> _repository;
        public AssignPlayerToTournamentCommandHandler(IAggregateRepository<Tournament> reposiotry)
        {
            _repository = reposiotry;
        }
        public async Task<Unit> Handle(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId);
            tournament.AddPlayer(request.Name, request.Surname);

            await _repository.SaveAsync(tournament);

            return Unit.Value;
        }
    }

}
