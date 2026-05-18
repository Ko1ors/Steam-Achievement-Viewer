using Microsoft.AspNetCore.Mvc;
using Sav.Common.Models;
using Sav.Common.Services;

namespace Sav.WebApp.Controllers
{
    public class AchievementsController : ExtendedControllerBase
    {
        private readonly IGameAchievementsService _gameAchievementsService;

        public AchievementsController(IGameAchievementsService gameAchievementsService)
        {
            _gameAchievementsService = gameAchievementsService;
        }

        [HttpGet("all")]
        public IActionResult GetAllAchievements()
        {
            return Ok(_gameAchievementsService.GetAchievementComposites());
        }

        [HttpGet("rarest")]
        public IActionResult GetRarestAchievements([FromQuery] int page = 1, [FromQuery] int count = 100)
        {
            return Ok(_gameAchievementsService.GetRarestAchievements(page, count));
        }

        [HttpGet("latest")]
        public IActionResult GetLatestAchievements([FromQuery] int page = 1, [FromQuery] int count = 100)
        {
            return Ok(_gameAchievementsService.GetLatestAchievements(page, count));
        }

        [HttpGet("stats/completed")]
        public IActionResult GetCompletedAchievementsCount()
        {
            return Ok(_gameAchievementsService.GetCompletedAchievementsCount());
        }

        [HttpGet("stats/total")]
        public IActionResult GetTotalAchievementsCount()
        {
            return Ok(_gameAchievementsService.GetTotalAchievementsCount());
        }
    }
}