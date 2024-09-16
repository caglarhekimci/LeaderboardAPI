namespace LeaderboardAPI.Models
{
    public class MatchResult
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public DateTime MatchDate { get; set; }
    }

}
