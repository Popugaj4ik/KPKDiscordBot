using Discord;

namespace DiscordBotSyriaRP.Logger
{
    public interface ILoger
    {
        public Task Log(LogMessage message);
    }
}