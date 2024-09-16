namespace LeaderboardAPI.Models
{
    public class LeaderboardEntry
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public int PlayerLevel { get; set; }
        public int TrophyCount { get; set; }
    }

}
