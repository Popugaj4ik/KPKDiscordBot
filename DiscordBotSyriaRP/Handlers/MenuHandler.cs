using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBotSyriaRP.Handlers
{
    public class MenuHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interaction;
        private readonly IServiceProvider _serviceProvider;

        public MenuHandler(DiscordSocketClient client, InteractionService interaction, IServiceProvider serviceProvider)
        {
            _client = client;
            _interaction = interaction;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            _client.SelectMenuExecuted += HandleMenuAsync;
        }

        public void AddModule<T>(IServiceProvider provider) where T : class
        {
            _interaction.AddModuleAsync<T>(provider);
        }

        private async Task HandleMenuAsync(SocketMessageComponent arg)
        {
            if (arg == null) return;

            var contex = new SocketInteractionContext(_client, arg);

            var res = await _interaction.ExecuteCommandAsync(contex, _serviceProvider);
        }
    }
}