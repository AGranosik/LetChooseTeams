using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Application.Tournaments
{
    public class queyr : IRequest<List<string>>
    {
        public string Name { get; set; }
    }

    public class queyrHandler : IRequestHandler<queyr, List<string>>
    {
        private readonly LctDbContext _dbContext;
        public queyrHandler(LctDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<string>> Handle(queyr request, CancellationToken cancellationToken)
        {
            return await _dbContext.Tournaments.Where(t => ((string)t.TournamentName).Contains(request.Name))
                .Select(t => t.TournamentName.ToString())
                .ToListAsync(cancellationToken);
        }
    }
}
