using Microsoft.AspNetCore.Mvc;
using Sav.Common.Services;

namespace Sav.WebApp.Controllers
{
    public class GamesController : ExtendedControllerBase
    {
        private readonly IGameAchievementsService _gameAchievementsService;

        public GamesController(IGameAchievementsService gameAchievementsService)
        {
            _gameAchievementsService = gameAchievementsService;
        }


        [HttpGet]
        public IActionResult GetCompletedGames(string steamId, int take = 25, int skip = 0)
        {
            return Ok(_gameAchievementsService.GetPagedCompletedGamesAsync(steamId, take, skip));
        }
    }
}
