using LCT.Application.Tournaments.Actions.TeamChooseAction;
using MediatR;

namespace LCT.Application.Teams.Events
{
    public class TeamSelectedActionHandler : INotificationHandler<TeamSelectedAction>
    {
        public Task Handle(TeamSelectedAction notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
