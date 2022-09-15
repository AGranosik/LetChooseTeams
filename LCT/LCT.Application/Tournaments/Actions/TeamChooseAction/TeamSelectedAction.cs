using LCT.Infrastructure.Repositories.Actions;

namespace LCT.Application.Tournaments.Actions.TeamChooseAction
{
    public class TeamSelectedAction : LctAction<Guid, string>
    {
        public string Team { get; set; }
    }
}
