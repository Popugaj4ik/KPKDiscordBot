using Discord;

namespace DiscordBotSyriaRP.Logger
{
    public class ConsoleLoger : Loger
    {
        public override async Task Log(LogMessage message)
        {
            Task.Run(() => LogToConsole(this, message));
        }

        private void LogToConsole(ConsoleLoger consoleLogger, LogMessage message)
        {
            Console.ForegroundColor = message.Severity switch
            {
                LogSeverity.Error => ConsoleColor.DarkRed,
                LogSeverity.Warning => ConsoleColor.DarkYellow,
                LogSeverity.Critical => ConsoleColor.Red,
                LogSeverity.Verbose => ConsoleColor.DarkGreen,
                _ => ConsoleColor.Gray,
            };
            Console.WriteLine($"{{{guid.ToString()[^4..]}}} : {message}");
        }
    }
}