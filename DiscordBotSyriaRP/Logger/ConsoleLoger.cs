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
            var time = DateTimeOffset.Now;
            Console.ForegroundColor = message.Severity switch
            {
                LogSeverity.Error => ConsoleColor.DarkRed,
                LogSeverity.Warning => ConsoleColor.DarkYellow,
                LogSeverity.Critical => ConsoleColor.Red,
                LogSeverity.Verbose => ConsoleColor.DarkGreen,
                _ => ConsoleColor.Gray,
            };
            Console.WriteLine($"{{{guid}}} {time.ToString("yyyy:MM:dd MMM-ddd HH:mm:ss.fff (zzz)")} {{{message.Severity}}}: {message}");
        }
    }
}