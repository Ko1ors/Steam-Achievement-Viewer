using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Common.Models;
using System;
using System.IO;
using System.Text.Json;

namespace Sav.Common.Services
{
    public class JsonSettingsService : ISettingsService
    {
        private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

        private readonly object _syncRoot = new();
        private readonly string _filePath;
        private UserSettings? _settings;

        public JsonSettingsService(string filePath)
        {
            _filePath = filePath;
        }

        public UserSettings Get()
        {
            lock (_syncRoot)
            {
                return _settings ??= Read();
            }
        }

        public void Save(UserSettings settings)
        {
            lock (_syncRoot)
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(_filePath, JsonSerializer.Serialize(settings, SerializerOptions));
                _settings = settings;
            }
        }

        private UserSettings Read()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new UserSettings();

                return JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(_filePath)) ?? new UserSettings();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Failed to read settings from {SettingsPath}", _filePath);
                return new UserSettings();
            }
        }
    }
}
