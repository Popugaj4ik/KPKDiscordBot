using Discord;
using System.Text.Json;

namespace DiscordBotSyriaRP.Extensions
{
    public static class Extentiones
    {
        public static string GetUserTag(this IUser User)
        {
            return $"{User.Username}#{User.Discriminator}";
        }

        public static Color FromJsonTODiscordColor(this string JsonArray)
        {
            try
            {
                var colors = JsonSerializer.Deserialize<int[]>(JsonArray);

                if (colors.Length < 3) return Color.Default;

                return new Color(colors[0], colors[1], colors[2]);
            }
            catch (Exception)
            {
                return Color.Default;
            }
        }
    }
}