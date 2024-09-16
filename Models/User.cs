namespace LeaderboardAPI.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int PlayerLevel { get; set; }
        public int TrophyCount { get; set; }
    }

}
