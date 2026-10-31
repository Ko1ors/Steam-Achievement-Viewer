using Microsoft.AspNetCore.Mvc;
using Sav.Common.Interfaces;
using Sav.Common.Models;
using Sav.Common.Models.DTOs;
using Sav.Common.Services;
using Sav.Infrastructure.Entities;

namespace Sav.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class SteamController : ExtendedControllerBase
    {
        private readonly ISteamService _steamService;
        private readonly IQueueService<UserGameEntity> _queueService;
        private readonly IGameAchievementsService _gameAchievementsService;

        public SteamController(ISteamService steamService, IQueueService<UserGameEntity> queueService, IGameAchievementsService gameAchievementsService)
        {
            _steamService = steamService;
            _queueService = queueService;
            _gameAchievementsService = gameAchievementsService;
        }

        [HttpGet("isLogged")]
        public IActionResult IsLogged()
        {
            return Ok(_steamService.IsLogged());
        }

        [HttpGet("currentUser")]
        public IActionResult GetCurrentUser()
        {
            return Ok(CreateCurrentUserDto());
        }

        [HttpPost("currentUser")]
        public IActionResult SetCurrentUser([FromBody] SetCurrentUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.SteamId))
                return BadRequest("Steam ID is required.");

            _steamService.LoadProfile(request.SteamId);
            return Ok(CreateCurrentUserDto());
        }

        private CurrentUserDto CreateCurrentUserDto()
        {
            var steamId = _steamService.GetUserId();

            return new CurrentUserDto
            {
                IsLoggedIn = _steamService.IsLogged(),
                SteamId = steamId,
                Profile = BuildProfile(steamId),
                QueueSize = _queueService.Size,
            };
        }

        private UserProfileDto? BuildProfile(string? steamId)
        {
            if (string.IsNullOrEmpty(steamId))
                return null;

            var user = _steamService.GetUser(steamId);
            if (user is null)
                return null;

            return new UserProfileDto(
                user,
                _steamService.GetUserGames(steamId).Count(),
                _gameAchievementsService.GetTotalAchievementsCount(steamId),
                _gameAchievementsService.GetCompletedAchievementsCount(steamId));
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            return Ok(_steamService.GetUser());
        }

        [HttpGet("profile")]
        public IActionResult GetProfile([FromQuery] string? steamId)
        {
            return Ok(BuildProfile(steamId ?? _steamService.GetUserId()));
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