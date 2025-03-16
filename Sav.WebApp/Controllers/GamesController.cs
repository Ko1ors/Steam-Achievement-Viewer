using Microsoft.AspNetCore.Mvc;
using Sav.Common.Models.DTOs;
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
        public async Task<IActionResult> GetCompletedGames(string steamId, int take = 25, int skip = 0)
        {
            return Ok(await _gameAchievementsService.GetPagedCompletedGamesAsync(steamId, take, skip));
        }

        [HttpGet]
        public IActionResult GetIncompleteGames(string steamId, int take = 25, int skip = 0)
        {
            return Ok(_gameAchievementsService.GetIncompleteGames(steamId, take, skip).Select(g => new GameDto(g)));
        }

        [HttpGet]
        public IActionResult GetGameClosestAchievements(string steamId, string appid)
        {
            return Ok(_gameAchievementsService.GetGameClosestAchievements(steamId, appid));
        }
    }
}
