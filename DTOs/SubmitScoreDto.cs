namespace LeaderboardAPI.DTOs
{
    public class SubmitScoreDto
    {
        public Guid UserId { get; set; }
        public int Score { get; set; }
    }
}
