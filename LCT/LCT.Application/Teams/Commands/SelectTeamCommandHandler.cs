using LCT.Application.Tournaments.Hubs;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entities.Tournaments.Types;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LCT.Application.Teams.Commands
{
    public class SelectTeamCommand: IRequest
    {
        public Guid PlayerId { get; set; }
        public Guid TournamentId { get; set; }
        public string Team { get; set; }
    }
    public class SelectTeamCommandHandler : IRequestHandler<SelectTeamCommand>
    {
        private readonly LctDbContext _dbContext;
        private readonly IHubContext<PlayerAssignedHub> _hubContext;
        public SelectTeamCommandHandler(LctDbContext dbContext, IHubContext<PlayerAssignedHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }
        public async Task<Unit> Handle(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments
                .Include(t => t.Players)
                .Include(t => t.SelectedTeams)
                .SingleOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);

            if (tournament == null)
                throw new ArgumentException("Turniej nie istnieje.");

            tournament.SelectTeam(request.PlayerId, request.Team);

            await _dbContext.SaveChangesAsync();
            try
            {
                await _hubContext.Clients.All.SendCoreAsync(request.TournamentId.ToString() + "/select", new[] { request.Team }); // finish tests
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return Unit.Value;
        }
    }
}
