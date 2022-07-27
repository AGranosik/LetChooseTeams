using LCT.Application.Players.Events;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IMediator _mediator;
        public AssignPlayerToTournamentCommandHandler(LctDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task<Guid> Handle(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            return Guid.Empty;
            //var tournament = await _dbContext.Tournaments.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == request.TournamentId, cancellationToken);
            //var player = Player.Register(new Name(request.Name), new Name(request.Surname));

            //tournament.AddPlayer(player);

            //await _dbContext.SaveChangesAsync(cancellationToken);

            //try
            //{
            //    await _mediator.Publish(new PlayerAssignedEvent
            //    {
            //        TournamentId = request.TournamentId,
            //        Name = request.Name,
            //        Surname = request.Surname,
            //        PlayerId = player.Id
            //    });

            //}
            //catch (Exception ex)
            //{

            //}
            //return player.Id;
        }
    }

}
