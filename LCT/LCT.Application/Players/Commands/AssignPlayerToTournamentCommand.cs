using MediatR;

namespace LCT.Application.Players.Commands
{
    public class AssignPlayerToTournamentCommand: IRequest
    {
        public string PlayerName { get; set; }
        public int TournamentId { get; set; }
    }


}
