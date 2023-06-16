using DiscordBotSyriaRP.Logger;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace DiscordBotSyriaRP.Configs
{
    public class ConfigWatcher<T>
    {
        private readonly string _configFileName;
        private readonly string _configFilePath;
        private readonly T _config;
        private readonly FileSystemWatcher _watcher;
        private readonly IServiceProvider service;

        public ConfigWatcher(IServiceProvider serviceProvider)
        {
            _configFileName = "DynamicConfig.json";
            _configFilePath = $"{Directory.GetCurrentDirectory()}\\{_configFileName}";
            _config = serviceProvider.GetRequiredService<T>();
            service = serviceProvider;
            if (_config == null)
            {
                throw new ArgumentNullException(nameof(_config));
            }

            _watcher = new FileSystemWatcher(Directory.GetCurrentDirectory(), _configFileName);
        }

        public async void Start()
        {
            _watcher.Changed += OnConfigFileChanged;
            _watcher.Renamed += OnConfigFileRenamed;
            _watcher.Deleted += OnConfigFileDeleted;
            _watcher.EnableRaisingEvents = true;
            await Log(new Discord.LogMessage(Discord.LogSeverity.Info, nameof(ConfigWatcher<T>), "Config watcher started"));
        }

        public async void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnConfigFileChanged;
            _watcher.Renamed -= OnConfigFileRenamed;
            _watcher.Deleted -= OnConfigFileDeleted;
            await Log(new Discord.LogMessage(Discord.LogSeverity.Info, nameof(ConfigWatcher<T>), "Config watcher stopped"));
        }

        private async void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var json = await File.ReadAllTextAsync(e.FullPath);
                JsonConvert.PopulateObject(json, _config);
            }
            catch (Exception ex)
            {
                await Log(new Discord.LogMessage(Discord.LogSeverity.Error, nameof(ConfigWatcher<T>), "Error when trying to load config"));
                return;
            }
            await Log(new Discord.LogMessage(Discord.LogSeverity.Info, nameof(ConfigWatcher<T>), $"Config file {e.Name} succesfuly applied"));
        }

        private async void OnConfigFileDeleted(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
            {
                await Log(new Discord.LogMessage(Discord.LogSeverity.Critical, nameof(ConfigWatcher<T>), "Config file deleted"));
                var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                await File.WriteAllTextAsync(e.FullPath, json);
                await Log(new Discord.LogMessage(Discord.LogSeverity.Info, nameof(ConfigWatcher<T>), "Config file restored"));
            }
            else
            {
                OnConfigFileChanged(sender, e);
            }
        }

        private async void OnConfigFileRenamed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == _configFilePath) return;

            await Log(new Discord.LogMessage(Discord.LogSeverity.Info, nameof(ConfigWatcher<T>), $"Config file renamed to {e.Name}"));
            try
            {
                File.Move(e.FullPath, _configFilePath, true);
                await Log(new Discord.LogMessage(Discord.LogSeverity.Info, nameof(ConfigWatcher<T>), $"Restoredfile into {_configFileName}"));
            }
            catch (Exception ex)
            {
                await Log(new Discord.LogMessage(Discord.LogSeverity.Warning, nameof(ConfigWatcher<T>), $"Failed to restore file"));
            }
        }

        private async Task Log(Discord.LogMessage logMessage)
        {
            await service.GetRequiredService<ILoger>().Log(logMessage);
        }
    }
}