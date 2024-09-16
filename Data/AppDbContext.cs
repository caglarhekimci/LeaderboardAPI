using LeaderboardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaderboardAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }
    }
}

