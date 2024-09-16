using Microsoft.AspNetCore.Mvc;
using LeaderboardAPI.Services;
using LeaderboardAPI.DTOs;

namespace LeaderboardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;

        public LeaderboardController(LeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore(SubmitScoreDto request)
        {
            var result = await _leaderboardService.SubmitScoreAsync(request);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet("top-scores")]
        public async Task<IActionResult> GetTopScores()
        {
            var result = await _leaderboardService.GetTopScoresAsync();
            return Ok(result);
        }
    }

}
