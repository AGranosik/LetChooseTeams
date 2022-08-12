using LCT.Application.Players.Events;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.Repositories;
using MediatR;
using Serilog;

namespace LCT.Application.Players.Commands
{
    public class AssignPlayerToTournamentCommand: IRequest<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid TournamentId { get; set; }
    }

    public record PlayerAssignedMessageDto(Guid Id, string Name, string Surname);
    public class AssignPlayerToTournamentCommandHandler : IRequestHandler<AssignPlayerToTournamentCommand, Guid>
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Tournament> _repository;
        public AssignPlayerToTournamentCommandHandler(IMediator mediator, IRepository<Tournament> reposiotry)
        {
            _mediator = mediator;
            _repository = reposiotry;
        }
        public async Task<Guid> Handle(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.Load(request.TournamentId);
            var playerId = tournament.AddPlayer(request.Name, request.Surname);

            await _repository.Save(tournament);
            try
            {
                await _mediator.Publish(new PlayerAssignedEvent
                {
                    TournamentId = request.TournamentId,
                    Name = request.Name,
                    Surname = request.Surname,
                    PlayerId = playerId
                });

            }
            catch(Exception ex)
            {
                Log.Error(ex, "While publishing player assigment event...");
            }
            return playerId;
        }
    }

}
