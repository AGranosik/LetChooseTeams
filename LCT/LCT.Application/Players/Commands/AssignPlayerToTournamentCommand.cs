using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Repositories;
using LCT.Core.Entites.Tournaments.ValueObjects;
using MediatR;

namespace LCT.Application.Players.Commands
{
    public class AssignPlayerToTournamentCommand: IRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid TournamentId { get; set; }
    }


    public class AssignPlayerToTournamentCommandHandler : IRequestHandler<AssignPlayerToTournamentCommand>
    {
        private readonly ITournamentRepository _repository;
        public AssignPlayerToTournamentCommandHandler(ITournamentRepository reposiotry)
        {
            _repository = reposiotry;
        }
        public async Task<Unit> Handle(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.GetTournament(request.TournamentId);
            var player = Player.Register(new Name(request.Name), new Name(request.Surname));

            tournament.AddPlayer(player);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

}
