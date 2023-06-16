using Discord;
using DiscordBotSyriaRP.Constants;

namespace DiscordBotSyriaRP.Logger
{
    public class FileLogger : Loger
    {
        private static object lockObject = new();

        public override async Task Log(LogMessage message)
        {
            Task.Run(() => LogToFile(this, message));
        }

        private async void LogToFile(FileLogger fileLogger, LogMessage message)
        {
            var time = DateTimeOffset.Now;
            lock (lockObject)
            {
                var writer = File.AppendText(GlobalConstants.LogFile);
                writer.WriteLine($"{{{guid}}} {time.ToString("yyyy:MM:dd MMM-ddd HH:mm:ss.fff (zzz)")} {{{message.Severity}}}: {message}");
                writer.Close();
            }
        }
    }
}