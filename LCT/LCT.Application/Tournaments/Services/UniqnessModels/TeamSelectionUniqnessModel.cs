namespace LCT.Application.Tournaments.Services.UniqnessModels
{
    class TeamSelectionUniqnessModel
    {
        public string playerName { get; set; }
        public string playerSurname { get; set; }
        public Guid TournamentId { get; set; }
    }
}
