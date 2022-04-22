using LCT.Application.Tournaments.Hubs;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        private readonly LctDbContext _dbContext;
        private readonly IHubContext<PlayerAssignedHub> _hubContext;
        public AssignPlayerToTournamentCommandHandler(LctDbContext dbContext, IHubContext<PlayerAssignedHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }
        public async Task<Guid> Handle(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);
            var player = Player.Register(new Name(request.Name), new Name(request.Surname));

            tournament.AddPlayer(player);
            await _dbContext.SaveChangesAsync(cancellationToken);
            try
            {
                await _hubContext.Clients.All.SendCoreAsync(tournament.Id.ToString(), new[] { new PlayerAssignedMessageDto(player.Id, player.Name.Value, player.Surname.Value) }, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception during signalr conntection");
            }
            return player.Id;
        }
    }

}
