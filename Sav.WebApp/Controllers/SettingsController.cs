using Microsoft.AspNetCore.Mvc;
using Sav.Common.Interfaces;
using Sav.Common.Models;
using Sav.Common.Models.DTOs;
using System.Text.RegularExpressions;

namespace Sav.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : ExtendedControllerBase
    {
        private const int MaxAutoUpdateIntervalMinutes = 7 * 24 * 60;
        private const string InvalidSteamIdMessage = "Enter a valid Steam ID: 17 digits starting with 7656.";

        private static readonly Regex SteamId64Pattern = new("^7656\\d{13}$", RegexOptions.Compiled);

        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public IActionResult GetSettings()
        {
            return Ok(CreateDto(_settingsService.Get()));
        }

        [HttpPut]
        public IActionResult UpdateSettings([FromBody] UpdateSettingsRequest? request)
        {
            if (request is null)
                return BadRequest("Settings are required.");

            var steamId = request.SteamId?.Trim();
            if (!string.IsNullOrEmpty(steamId) && !SteamId64Pattern.IsMatch(steamId))
                return BadRequest(InvalidSteamIdMessage);

            var steamApiKey = request.SteamApiKey?.Trim();

            var settings = _settingsService.Get();
            settings.SteamID = string.IsNullOrEmpty(steamId) ? null : steamId;
            settings.SteamApiKey = string.IsNullOrEmpty(steamApiKey) ? null : steamApiKey;
            settings.AutoUpdateIntervalMinutes = NormalizeInterval(request.AutoUpdateIntervalMinutes);

            _settingsService.Save(settings);

            return Ok(CreateDto(settings));
        }

        private static int NormalizeInterval(int minutes)
        {
            if (minutes < 1)
                return UserSettings.DefaultAutoUpdateIntervalMinutes;

            return Math.Min(minutes, MaxAutoUpdateIntervalMinutes);
        }

        private static SettingsDto CreateDto(UserSettings settings)
        {
            return new SettingsDto
            {
                SteamId = settings.SteamID,
                SteamApiKey = settings.SteamApiKey,
                AutoUpdateIntervalMinutes = settings.AutoUpdateIntervalMinutes,
            };
        }
    }
}
