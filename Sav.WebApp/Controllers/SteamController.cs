using Microsoft.AspNetCore.Mvc;
using Sav.Common.Interfaces;
using Sav.Common.Models;
using Sav.Common.Services;
using Sav.Infrastructure.Entities;

namespace Sav.WebApp.Controllers
{
    public class SteamController : ExtendedControllerBase
    {
        private readonly ISteamService _steamService;
        private readonly IQueueService<UserGameEntity> _queueService;

        public SteamController(ISteamService steamService, IQueueService<UserGameEntity> queueService)
        {
            _steamService = steamService;
            _queueService = queueService;
        }

        [HttpGet("isLogged")]
        public IActionResult IsLogged()
        {
            return Ok(_steamService.IsLogged());
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            return Ok(_steamService.GetUser());
        }

        [HttpGet("userId")]
        public IActionResult GetUserId()
        {
            return Ok(_steamService.GetUserId());
        }

        [HttpGet("userGames")]
        public IActionResult GetUserGames()
        {
            return Ok(_steamService.GetUserGames());
        }

        [HttpPost("loadProfile")]
        public IActionResult LoadProfile([FromBody] string steamId)
        {
            _steamService.LoadProfile(steamId);
            return Ok();
        }

        [HttpPost("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] string steamId)
        {
            var result = await _steamService.UpdateProfileAsync(steamId);
            return Ok(result);
        }

        [HttpPost("updateGames")]
        public async Task<IActionResult> UpdateGames([FromBody] string steamId)
        {
            var result = await _steamService.UpdateGamesAsync(steamId);
            return Ok(result);
        }

        [HttpPost("queueAchievementsUpdate")]
        public IActionResult QueueAchievementsUpdate([FromQuery] bool onlyRecentGames = true)
        {
            _steamService.QueueAchievementsUpdate(onlyRecentGames);
            return Ok();
        }

        [HttpPost("saveSettings")]
        public IActionResult SaveSettingsInfo()
        {
            _steamService.SaveSettingsInfo();
            return Ok();
        }

        [HttpGet("queueSize")]
        public IActionResult GetQueueSize()
        {
            return Ok(_queueService.Size);
        }
    }
}