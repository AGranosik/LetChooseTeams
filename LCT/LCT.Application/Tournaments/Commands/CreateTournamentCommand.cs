using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Services;
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
        private readonly ITournamentDomainService _tournamentService;
        public CreateTournamentCommandHandler(IRepository<Tournament> repository, ITournamentDomainService domainService)
        {
            _repository = repository;
            _tournamentService = domainService;
        }

        public async Task<Guid> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = Tournament.Create(request.Name, request.PlayerLimit);
            await _tournamentService.ValidateAsync(tournament); // Czy nie przeniesc tego gdzieś, bo wtedy mozna tego nie uzyc i nie wymuszam tego na devie...
            await _repository.Save(tournament);
            return tournament.GetChanges().Last().StreamId;
        }
    }
}
