using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
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
            await _tournamentService.TournamentUniqnessValidationAsync(tournament); 
            await _repository.SaveAsync(tournament);
            return tournament.Id.Value;
        }
    }
}
