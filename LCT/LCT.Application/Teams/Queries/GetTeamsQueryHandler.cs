using LCT.Core.Entities.Tournaments.Types;
using MediatR;

namespace LCT.Application.Teams.Queries
{
    public class GetTeamsQuery : IRequest<List<string>>
    {

    }
    public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, List<string>>
    {
        public async Task<List<string>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
            => TournamentTeamNames._teams;
    }
}
