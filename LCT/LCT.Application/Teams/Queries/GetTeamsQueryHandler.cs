﻿using MediatR;

namespace LCT.Application.Teams.Queries
{
    public record TeamToSelectDto(string Name, bool Selected);
    public class GetTeamsQuery : IRequest<List<TeamToSelectDto>>
    {
        public Guid TournamentId { get; set; }
    }
    public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, List<TeamToSelectDto>>
    {
        public GetTeamsQueryHandler()
        {
        }
        public async Task<List<TeamToSelectDto>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            return null;
            //var alreadySelected = await _dbContext.Tournaments.Where(t => t.Id == request.TournamentId)
            //    .SelectMany(t => t.SelectedTeams).Select(st => st.TeamName).ToListAsync();

            //return TournamentTeamNames.Teams.Select(t => new TeamToSelectDto(t, alreadySelected.Any(selected => selected.Value == t))).ToList();
        }
        
    }
}
