using Discord.Interactions;
using Discord.WebSocket;
using DiscordBotSyriaRP.Configs;
using DiscordBotSyriaRP.Constants;
using DiscordBotSyriaRP.EF;
using DiscordBotSyriaRP.Modals;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscordBotSyriaRP.Modules.ModalModules
{
    public class HandleModalCallbackButton : InteractionModuleBase<SocketInteractionContext>
    {
        private Config Config;

        public HandleModalCallbackButton(IServiceProvider provider)
        {
            Config = provider.GetRequiredService<Config>();
            DB = provider.GetRequiredService<KPKContext>();

            if (Config.ChatChannelID == null || Config.LogChannelID == null)
            {
                throw new NullReferenceException("There is no channels in a config");
            }
        }

        [ComponentInteraction(ComponentsIDs.ModalCallMenuID, runMode: Discord.Interactions.RunMode.Async)]
        public async Task HandleModalCallButtonClick()
        {
            var modalType = new List<PropertyInfo>(Context.Interaction.GetType().GetProperties())
                .Where(x => x.PropertyType == typeof(SocketMessageComponentData))
                .Select(x => (SocketMessageComponentData?)x.GetValue(Context.Interaction))
                .Select(x => x.Values.First())
                .First()
                ;

            switch (modalType)
            {
                case GlobalConstants.Anon:
                    await SendAnonModal();
                    break;

                case GlobalConstants.Msg:
                    await SendMessageModal();
                    break;

                case GlobalConstants.Avatar:
                    await SendAvatarChangeModal();
                    break;

                default:
                    await RespondAsync("Error");
                    break;
            }
        }

        private async Task SendAnonModal()
        {
            if (Config.Admins.Any(x => x == Context.User.Id))
            {
                await RespondWithModalAsync<AnonAdminModal>(ComponentsIDs.AnonAdminModalID);
                return;
            }

            await RespondWithModalAsync<AnonModal>(ComponentsIDs.AnonModalID);
        }

        private async Task SendMessageModal()
        {
            await RespondWithModalAsync<MessageModal>(ComponentsIDs.MessageModalID);
        }

        private async Task SendAvatarChangeModal()
        {
            await RespondWithModalAsync<AvatarChangeModal>(ComponentsIDs.AvatarChangeModalID);
        }
    }
}