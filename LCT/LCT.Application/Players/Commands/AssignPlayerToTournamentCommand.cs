using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Common.Interfaces;
using MediatR;

namespace LCT.Application.Players.Commands
{
    public class AssignPlayerToTournamentCommand: IRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid TournamentId { get; set; }
    }

    public record PlayerAssignedMessageDto(Guid Id, string Name, string Surname);
    public class AssignPlayerToTournamentCommandHandler : IRequestHandler<AssignPlayerToTournamentCommand>
    {
        private readonly IAggregateRepository<Tournament> _repository;
        public AssignPlayerToTournamentCommandHandler(IAggregateRepository<Tournament> reposiotry)
        {
            _repository = reposiotry;
        }
        public async Task Handle(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId, cancellationToken);
            tournament.AddPlayer(request.Name, request.Surname);
            await _repository.SaveAsync(tournament);
        }
    }

}
