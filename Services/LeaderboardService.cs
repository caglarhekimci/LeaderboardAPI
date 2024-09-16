using LeaderboardAPI.Data;
using LeaderboardAPI.DTOs;
using LeaderboardAPI.Models;
using StackExchange.Redis;

namespace LeaderboardAPI.Services
{
    public class LeaderboardService
    {
        private readonly AppDbContext _context;
        private readonly IConnectionMultiplexer _redis;

        public LeaderboardService(AppDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
        }
        //SubmitScoreAsync metodu, kullanıcının skorunu Redis'e sıralı bir set (sorted set) olarak ekler.
        public async Task<ServiceResponse<string>> SubmitScoreAsync(SubmitScoreDto request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                // Skoru Redis'e kaydediyoruz
                var db = _redis.GetDatabase();
                await db.SortedSetAddAsync("leaderboard", request.UserId.ToString(), request.Score);

                // Ayrıca MySQL'e kalıcı olarak kaydediyoruz
                var matchResult = new MatchResult
                {
                    UserId = request.UserId,
                    Score = request.Score,
                    MatchDate = DateTime.UtcNow
                };

                _context.MatchResults.Add(matchResult);
                await _context.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            return new ServiceResponse<string> { Success = true, Message = "Score submitted successfully." };
        }


        //GetTopScoresAsync metodu, Redis üzerinden en iyi 100 oyuncuyu alır ve onları geri döner.
        public async Task<List<LeaderboardEntry>> GetTopScoresAsync()
        {
            var db = _redis.GetDatabase();
            var topScores = await db.SortedSetRangeByRankWithScoresAsync("leaderboard", 0, 99);

            var result = new List<LeaderboardEntry>();

            foreach (var entry in topScores)
            {
                var userId = Guid.Parse(entry.Element);
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    result.Add(new LeaderboardEntry
                    {
                        Username = user.Username,
                        Score = (int)entry.Score,
                        PlayerLevel = user.PlayerLevel,
                        TrophyCount = user.TrophyCount
                    });
                }
            }

            return result;
        }
    }

}

//Redis: Gerçek zamanlı ve hızlı erişim gerektiren veriler (örneğin, liderlik tablosu).
//MySQL: Kalıcı ve daha az sıklıkla erişilen veriler (örneğin, kullanıcı bilgileri, geçmiş maç skorları).