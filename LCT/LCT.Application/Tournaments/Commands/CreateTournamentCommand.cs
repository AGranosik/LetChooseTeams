using LCT.Core.Entites.Tournaments.Entities;
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
        private readonly IRepository<Tournament> _repository;
        public CreateTournamentCommandHandler(IRepository<Tournament> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            // find a way to check uniqness of name
            // db constraint isnt great solution because there can be more than per entity
            // proxy collection? But how the fuck implement that
            var tournament = Tournament.Create(request.Name, request.PlayerLimit);
            await _repository.Save(tournament);
            return tournament.GetChanges().Last().StreamId;
        }
    }
}
