using LCT.Infrastructure.EF;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
