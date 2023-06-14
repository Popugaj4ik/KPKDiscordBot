using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSyriaRP.Configs;
using DiscordBotSyriaRP.Constants;
using DiscordBotSyriaRP.EF;
using DiscordBotSyriaRP.Extensions;
using DiscordBotSyriaRP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace DiscordBotSyriaRP.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private Config Config;
        private DiscordSocketClient client;
        private KPKContext DB;

        public HelpModule(IServiceProvider provider)
        {
            Config = provider.GetRequiredService<Config>();
            client = provider.GetRequiredService<DiscordSocketClient>();
            DB = provider.GetRequiredService<KPKContext>();
        }

        [Command("help", true, RunMode = RunMode.Async)]
        public async Task HandleHelpCommand()
        {
            var isUserExist = await DB.Users.AnyAsync(x => x.DiscordID == Context.User.Id);

            if (!isUserExist)
            {
                await CreateUser();
            }

            var menu = new SelectMenuBuilder()
                .WithCustomId(ComponentsIDs.ModalCallMenuID)
                .WithPlaceholder("Что ты хочешь сделать?")
                ;

            if (Config.IsIconsInMessageAllowed)
            {
                menu.AddOption("Поменять аватар сообщения", GlobalConstants.Avatar);
            }

            menu.AddOption("Написать анонимное сообщение", GlobalConstants.Anon)
                .AddOption("Написать сообщение от себя", GlobalConstants.Msg)
                ;

            var component = new ComponentBuilder()
                .WithSelectMenu(menu)
                ;

            var embed = await GetUserEmbed();

            if (isUserExist)
            {
                await Context.User.SendMessageAsync("Пожалуйста", components: component.Build());
            }
            else
            {
                await Context.User.SendMessageAsync("Пожалуйста, кстати, ты в системе первы раз, заодно посмотри как будут выглядеть твои сообщения", components: component.Build(), embed: embed);
            }
        }

        private async Task<Embed> GetUserEmbed()
        {
            var User = await DB.Users.FirstOrDefaultAsync(x => x.DiscordID == Context.User.Id);

            var msg = new StringBuilder()
                .AppendLine($"{GlobalConstants.MessageFrom} {GlobalConstants.GetUser(Context.User.Id.ToString())}")
                .AppendLine($"{GlobalConstants.MessageTo} Жлоб")
                .AppendLine($"{GlobalConstants.MessageContent} Эй, жлоб! Где туз? Прячь юных съёмщиц в шкаф")
                ;

            var embed = new EmbedBuilder()
                .WithDescription(msg.ToString())
                .WithThumbnailUrl(User?.AvatarLink)
                .WithColor(Color.Default)
                ;

            return embed.Build();
        }

        private async Task CreateUser()
        {
            var User = new User()
            {
                DiscordID = Context.User.Id,
                DisordTag = Context.User.GetUserTag(),
                AvatarLink = Config.IsIconsInMessageAllowed ? Config.DefaultIconLink : string.Empty,
                Created = DateTimeOffset.Now,
                Updated = DateTimeOffset.Now,
            };

            DB.Users.Add(User);

            await DB.SaveChangesAsync();
        }

        private async Task SendEmbed(Embed embed)
        {
            try
            {
                await Context.User.SendMessageAsync(embed: embed);
            }
            catch (Exception ex)
            {
                var newEmbed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithAuthor("Извините, но не получилось предоставить вам помошь.")
                    .WithDescription(ex.ToString())
                    .Build();

                await Context.User.SendMessageAsync(embed: newEmbed);
            }
        }
    }
}