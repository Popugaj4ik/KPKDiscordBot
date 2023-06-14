using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBotSyriaRP.Handlers
{
    public class ModalHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interaction;
        private readonly IServiceProvider _serviceProvider;

        public ModalHandler(DiscordSocketClient client, InteractionService interaction, IServiceProvider serviceProvider)
        {
            _client = client;
            _interaction = interaction;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            _client.ModalSubmitted += HandleModalAsync;
        }

        public void AddModule<T>(IServiceProvider provider) where T : class
        {
            _interaction.AddModuleAsync<T>(provider);
        }

        private async Task HandleModalAsync(SocketModal arg)
        {
            if (arg == null) return;

            var context = new SocketInteractionContext(_client, arg);

            var res = await _interaction.ExecuteCommandAsync(context, _serviceProvider);
        }
    }
}