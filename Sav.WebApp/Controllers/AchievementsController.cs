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

        [HttpGet()]
        public IActionResult GetAllAchievements()
        {
            return Ok(_gameAchievementsService.GetAchievementComposites());
        }

        [HttpGet()]
        public IActionResult GetRarestAchievements([FromQuery] string? steamId, [FromQuery] int page = 1, [FromQuery] int count = 100)
        {
            return Ok(_gameAchievementsService.GetRarestAchievements(page, count, steamId));
        }

        [HttpGet()]
        public IActionResult GetLatestAchievements([FromQuery] string? steamId, [FromQuery] int page = 1, [FromQuery] int count = 100)
        {
            return Ok(_gameAchievementsService.GetLatestAchievements(page, count, steamId));
        }

        [HttpGet()]
        public IActionResult GetCompletedAchievementsCount()
        {
            return Ok(_gameAchievementsService.GetCompletedAchievementsCount());
        }

        [HttpGet()]
        public IActionResult GetTotalAchievementsCount()
        {
            return Ok(_gameAchievementsService.GetTotalAchievementsCount());
        }
    }
}