using Discord;
using DiscordBotSyriaRP.Constants;

namespace DiscordBotSyriaRP.Logger
{
    public class FileLogger : Loger
    {
        public override async Task Log(LogMessage message)
        {
            LogToFile(this, message);
        }

        private async void LogToFile(FileLogger fileLogger, LogMessage message)
        {
            var writer = File.AppendText(GlobalConstants.LogFile);
            writer.WriteLine($"{{{guid.ToString()[^4..]}}} : {message}");
            writer.Close();
        }
    }
}