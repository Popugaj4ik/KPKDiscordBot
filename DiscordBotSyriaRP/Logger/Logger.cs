using Discord;

namespace DiscordBotSyriaRP.Logger
{
    public abstract class Loger : ILoger
    {
        public Guid guid;

        public Loger()
        {
            guid = Guid.NewGuid();
        }

        public abstract Task Log(LogMessage message);
    }
}