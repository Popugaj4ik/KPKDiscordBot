using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBotSyriaRP.Configs;
using DiscordBotSyriaRP.Constants;
using DiscordBotSyriaRP.EF;
using DiscordBotSyriaRP.Extensions;
using DiscordBotSyriaRP.Logger;
using DiscordBotSyriaRP.Modals;
using DiscordBotSyriaRP.Models;
using DiscordBotSyriaRP.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace DiscordBotSyriaRP.Modules
{
    public class ModalModule : InteractionModuleBase<SocketInteractionContext>
    {
        private Loger Loger;
        private Config Config;
        private DiscordSocketClient client;
        private MessageEncryptService MessageEncryptService;
        private KPKContext DB;

        public ModalModule(IServiceProvider provider)
        {
            Loger = provider.GetRequiredService<ConsoleLoger>();
            client = provider.GetRequiredService<DiscordSocketClient>();
            Config = provider.GetRequiredService<Config>();
            MessageEncryptService = provider.GetRequiredService<MessageEncryptService>();
            DB = provider.GetRequiredService<KPKContext>();

            if (Config.ChatChannelID == null || Config.LogChannelID == null)
            {
                throw new NullReferenceException("There is no channels in a config");
            }
        }

        [ModalInteraction(ComponentsIDs.AnonModalID, true, runMode: RunMode.Async)]
        public async Task HandleAnonModal(AnonModal anonModal)
        {
            var ChatChannel = (IMessageChannel)await client.GetChannelAsync(Config.ChatChannelID);
            var LogChannel = (IMessageChannel)await client.GetChannelAsync(Config.LogChannelID);

            if (ChatChannel == null || LogChannel == null)
            {
                await Loger.Log(new LogMessage(LogSeverity.Critical, "App", "Unable to obrain cahnnels"));
                throw new NullReferenceException("Channels not found");
            }

            var usernameWithoutTags = Context.User.Username;

            var random = new Random(usernameWithoutTags.GetHashCode() ^ (int)DateTime.Now.Ticks);

            var cleanMessage = anonModal.Message;

            if (Math.Round(random.NextDouble(), Config.ProbabilityRounding) < Config.ProbabilityOfNoiseAppearing)
            {
                cleanMessage = await MessageEncryptService.MakeNoise(usernameWithoutTags, cleanMessage);
            }

            var userRoleToTag = string.Empty;

            if (anonModal.SendToRole != string.Empty)
            {
                userRoleToTag = UInt64.TryParse(anonModal.SendToRole, out _) ? GlobalConstants.GetRole(anonModal.SendToRole) : anonModal.SendToRole;
            }
            else if (anonModal.SendToUser != string.Empty)
            {
                userRoleToTag = UInt64.TryParse(anonModal.SendToUser, out _) ? GlobalConstants.GetUser(anonModal.SendToUser) : anonModal.SendToUser;
            }
            else
            {
                userRoleToTag = GlobalConstants.MessageToAll;
            }

            var msg = new StringBuilder()
                .AppendLine($"{GlobalConstants.MessageFrom} {GlobalConstants.Anonim}")
                .AppendLine($"{GlobalConstants.MessageTo} {userRoleToTag}")
                .AppendLine($"{GlobalConstants.MessageContent} {cleanMessage}")
                ;

            var embed = new EmbedBuilder()
                .WithDescription(msg.ToString())
                ;

            if (Config.IsIconsInMessageAllowed)
            {
                embed.WithThumbnailUrl(Config.DefaultIconLink ?? Context.User.GetDefaultAvatarUrl());
            }

            await ChatChannel.SendMessageAsync(embed: embed.Build());

            embed.WithAuthor(Context.User);

            await LogChannel.SendMessageAsync(embed: embed.Build());

            await RespondAsync(embed: embed.Build(), allowedMentions: AllowedMentions.All);
        }

        [ModalInteraction(ComponentsIDs.MessageModalID, true, runMode: RunMode.Async)]
        public async Task HandleMessageModal(MessageModal messageModal)
        {
            var ChatChannel = (IMessageChannel)await client.GetChannelAsync(Config.ChatChannelID);
            var LogChannel = (IMessageChannel)await client.GetChannelAsync(Config.LogChannelID);

            if (ChatChannel == null || LogChannel == null)
            {
                await Loger.Log(new LogMessage(LogSeverity.Critical, "App", "Unable to obrain cahnnels"));
                throw new NullReferenceException("Channels not found");
            }

            var usernameWithoutTags = Context.User.Username;

            var random = new Random(usernameWithoutTags.GetHashCode() ^ (int)DateTime.Now.Ticks);

            var cleanMessage = messageModal.Message;

            if (Math.Round(random.NextDouble(), Config.ProbabilityRounding) < Config.ProbabilityOfNoiseAppearing)
            {
                cleanMessage = await MessageEncryptService.MakeNoise(usernameWithoutTags, cleanMessage);
            }

            var userRoleToTag = string.Empty;

            if (messageModal.SendToRole != string.Empty)
            {
                userRoleToTag = UInt64.TryParse(messageModal.SendToRole, out _) ? GlobalConstants.GetRole(messageModal.SendToRole) : messageModal.SendToRole;
            }
            else if (messageModal.SendToUser != string.Empty)
            {
                userRoleToTag = UInt64.TryParse(messageModal.SendToUser, out _) ? GlobalConstants.GetUser(messageModal.SendToUser) : messageModal.SendToUser;
            }
            else
            {
                userRoleToTag = GlobalConstants.MessageToAll;
            }

            var msg = new StringBuilder()
                .AppendLine($"{GlobalConstants.MessageFrom} {GlobalConstants.GetUser(Context.User.Id.ToString())}")
                .AppendLine($"{GlobalConstants.MessageTo} {userRoleToTag}")
                .AppendLine($"{GlobalConstants.MessageContent} {cleanMessage}")
                ;

            var embed = new EmbedBuilder()
                .WithDescription(msg.ToString())
                ;

            if (Config.IsIconsInMessageAllowed)
            {
                var user = await DB.Users.FirstOrDefaultAsync(x => x.DiscordID == Context.User.Id);

                if (user != null)
                {
                    embed.WithThumbnailUrl(user.AvatarLink);
                }
                else
                {
                    var userIconURL = Config.UserGroups.Where(x => x.Users.Any(y => y == Context.User.Id)).Select(x => x.GroupImage).FirstOrDefault();

                    var isUri = Uri.TryCreate(userIconURL, UriKind.Absolute, out var uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    embed.WithThumbnailUrl(isUri ? userIconURL : Config.DefaultIconLink ?? Context.User.GetDefaultAvatarUrl());
                }
            }

            var userID = Context.User.Id;

            if (Config.UserGroups.Any(x => x.Users.Any(y => y == userID)))
            {
                var colorBytes = Config.UserGroups.Where(x => x.Users.Any(y => y == userID)).Select(x => x.Color).First();
                var embedColor = FromByte(colorBytes);
                embed.WithColor(embedColor);
            }

            await ChatChannel.SendMessageAsync(embed: embed.Build());

            embed.WithAuthor(Context.User);

            await LogChannel.SendMessageAsync(embed: embed.Build());

            await RespondAsync(embed: embed.Build(), allowedMentions: AllowedMentions.All);
        }

        [ModalInteraction(ComponentsIDs.AnonAdminModalID, true, runMode: RunMode.Async)]
        public async Task HandleAnonAdminModal(AnonAdminModal anonAdminModal)
        {
            var ChatChannel = (IMessageChannel)await client.GetChannelAsync(Config.ChatChannelID);
            var LogChannel = (IMessageChannel)await client.GetChannelAsync(Config.LogChannelID);

            if (ChatChannel == null || LogChannel == null)
            {
                await Loger.Log(new LogMessage(LogSeverity.Critical, "App", "Unable to obrain cahnnels"));
                throw new NullReferenceException("Channels not found");
            }

            var usernameWithoutTags = Context.User.Username;

            var random = new Random(usernameWithoutTags.GetHashCode() ^ (int)DateTime.Now.Ticks);

            var cleanMessage = anonAdminModal.Message;

            if (Math.Round(random.NextDouble(), Config.ProbabilityRounding) < Config.ProbabilityOfNoiseAppearing)
            {
                cleanMessage = await MessageEncryptService.MakeNoise(usernameWithoutTags, cleanMessage);
            }

            var userRoleToTag = string.Empty;

            if (anonAdminModal.SendToRole != string.Empty)
            {
                userRoleToTag = UInt64.TryParse(anonAdminModal.SendToRole, out _) ? GlobalConstants.GetRole(anonAdminModal.SendToRole) : anonAdminModal.SendToRole;
            }
            else if (anonAdminModal.SendToUser != string.Empty)
            {
                userRoleToTag = UInt64.TryParse(anonAdminModal.SendToUser, out _) ? GlobalConstants.GetUser(anonAdminModal.SendToUser) : anonAdminModal.SendToUser;
            }
            else
            {
                userRoleToTag = GlobalConstants.MessageToAll;
            }

            var isImpersonate = anonAdminModal.AsPerson != string.Empty;

            var msg = new StringBuilder()
                .AppendLine($"{GlobalConstants.MessageFrom} {(isImpersonate ? anonAdminModal.AsPerson : GlobalConstants.Anonim)}")
                .AppendLine($"{GlobalConstants.MessageTo} {userRoleToTag}")
                .AppendLine($"{GlobalConstants.MessageContent} {cleanMessage}")
                ;

            var embed = new EmbedBuilder()
                .WithDescription(msg.ToString())
                ;

            if (Config.IsIconsInMessageAllowed)
            {
                embed.WithThumbnailUrl(anonAdminModal.AsPersonAvatar == string.Empty ? Config.DefaultIconLink ?? Context.User.GetDefaultAvatarUrl() : anonAdminModal.AsPersonAvatar);
            }

            if (isImpersonate && Config.CharactersColors.Any(x => x.Key == anonAdminModal.AsPerson))
            {
                embed.WithColor(FromByte(Config.CharactersColors[anonAdminModal.AsPerson]));
            }

            await ChatChannel.SendMessageAsync(embed: embed.Build());

            embed.WithAuthor(Context.User);

            await LogChannel.SendMessageAsync(embed: embed.Build());

            await RespondAsync(embed: embed.Build(), allowedMentions: AllowedMentions.All);
        }

        [ModalInteraction(ComponentsIDs.AvatarChangeModalID, true, runMode: RunMode.Async)]
        public async Task HandleAvatarChangeModal(AvatarChangeModal avatarChangeModal)
        {
            var User = await DB.Users.FirstOrDefaultAsync(x => x.DiscordID == Context.User.Id);

            if (User != null)
            {
                User.AvatarLink = avatarChangeModal.NewAvatarURL;
                User.Updated = DateTimeOffset.Now;
                User.DisordTag = Context.User.GetUserTag();
                DB.Users.Update(User);
            }
            else
            {
                User = new Models.User()
                {
                    DiscordID = Context.User.Id,
                    DisordTag = Context.User.GetUserTag(),
                    AvatarLink = avatarChangeModal.NewAvatarURL,
                    Created = DateTimeOffset.Now,
                    Updated = DateTimeOffset.Now,
                };
                DB.Users.Add(User);
            }

            await DB.SaveChangesAsync();

            var msg = new StringBuilder()
                .AppendLine($"{GlobalConstants.MessageFrom} {GlobalConstants.GetUser(Context.User.Id.ToString())}")
                .AppendLine($"{GlobalConstants.MessageTo} Жлоб")
                .AppendLine($"{GlobalConstants.MessageContent} Эй, жлоб! Где туз? Прячь юных съёмщиц в шкаф")
                ;

            var embed = new EmbedBuilder()
                .WithDescription(msg.ToString())
                .WithThumbnailUrl(avatarChangeModal.NewAvatarURL)
                ;

            if (Config.UserGroups.Any(x => x.Users.Any(y => y == User.DiscordID)))
            {
                var colorBytes = Config.UserGroups.Where(x => x.Users.Any(y => y == User.DiscordID)).Select(x => x.Color).First();
                var embedColor = FromByte(colorBytes);
                embed.WithColor(embedColor);
            }

            await RespondAsync("Теперь сообщения будут выглядить вот так:", embed: embed.Build());
        }

        private Color FromByte(byte[] data)
        {
            if (data.Length < 3) return Color.Default;

            return new Color(data[0], data[1], data[2]);
        }
    }
}