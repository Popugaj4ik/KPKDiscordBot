using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSyriaRP.Configs;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBotSyriaRP.Handlers
{
    public class Prefixhandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _command;
        private readonly IServiceProvider _serviceProvider;

        public Prefixhandler(DiscordSocketClient client, CommandService command, IServiceProvider serviceProvider)
        {
            _client = client;
            _command = command;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
        }

        public void AddModule<T>(IServiceProvider provider)
        {
            _command.AddModuleAsync<T>(provider);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            var AppConfig = _serviceProvider.GetRequiredService<DynamicConfig>();

            if (!(message.HasCharPrefix(AppConfig.Prefix, ref argPos)
                || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                || message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            var res = await _command.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: _serviceProvider
                );
        }
    }
}