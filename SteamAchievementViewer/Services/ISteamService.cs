﻿using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamAchievementViewer.Services
{
    public delegate void AchievementProgressUpdatedDelegate(int totalGames, int currentGameCount, string lastGameName);

    public delegate void AvatarUpdatedDelegate(string avatarUrl);

    public interface ISteamService
    {
        public event AchievementProgressUpdatedDelegate OnAchievementProgressUpdated;
        
        public event AvatarUpdatedDelegate OnAvatarUpdated;

        public UserEntity GetUser();

        public IEnumerable<GameEntity> GetUserGames();

        bool Start();

        bool IsLogged();

        Task<bool> UpdateProfileAsync(string steamID);

        Task<bool> UpdateGamesAsync(string steamID);

        void LoadProfile(string steamID);

        void SaveSettingsInfo();

    }
}
