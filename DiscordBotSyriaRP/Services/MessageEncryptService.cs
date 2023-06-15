using DiscordBotSyriaRP.Configs;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace DiscordBotSyriaRP.Services
{
    public class MessageEncryptService
    {
        private DynamicConfig Config;

        public MessageEncryptService(IServiceProvider provider)
        {
            Config = provider.GetRequiredService<DynamicConfig>();
        }

        public async Task<string> MakeNoise(string Username, string msg)
        {
            var random = new Random(Username.GetHashCode() ^ (int)DateTime.Now.Ticks);

            var returnValue = new StringBuilder();

            bool isNoised = false;

            int amountOfSpaces = 0, startAmountOfSpaces = 0;

            for (int i = 0; i < msg.Length; i++)
            {
                if (!isNoised)
                {
                    if (Math.Round(random.NextDouble(), Config.ProbabilityRounding) < Config.ProbabilityOfNoiseInMessage)
                    {
                        isNoised = !isNoised;
                        amountOfSpaces = random.Next(Config.MinAmountOfErasedLetters, Config.MaxAmountOfErasedLetters);
                        startAmountOfSpaces = amountOfSpaces;
                    }
                }
                else
                {
                    if (amountOfSpaces == startAmountOfSpaces)
                    {
                        returnValue.Append("...");
                        amountOfSpaces--;
                        continue;
                    }

                    if (amountOfSpaces != 0)
                    {
                        amountOfSpaces--;
                        continue;
                    }

                    if (amountOfSpaces == 0)
                    {
                        returnValue.Append(" ...");
                        isNoised = !isNoised;
                        continue;
                    }
                }

                returnValue.Append(msg[i]);
            }

            return returnValue.ToString();
        }
    }
}