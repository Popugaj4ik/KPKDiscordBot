namespace DiscordBotSyriaRP.Configs
{
    public class DynamicConfig
    {
        public char Prefix { get; set; }
        public UInt64 ChatChannelID { get; set; }
        public UInt64 LogChannelID { get; set; }
        public double ProbabilityOfNoiseAppearing { get; set; }
        public double ProbabilityOfNoiseInMessage { get; set; }
        public int ProbabilityRounding { get; set; }
        public int MinAmountOfErasedLetters { get; set; }
        public int MaxAmountOfErasedLetters { get; set; }
        public bool IsIconsInMessageAllowed { get; set; }
        public string? DefaultIconLink { get; set; }
        public UInt64[] Admins { get; set; } = Array.Empty<UInt64>();
        public Dictionary<string, int[]> CharactersColors { get; set; } = new Dictionary<string, int[]>();
        public UserGroup[] UserGroups { get; set; } = Array.Empty<UserGroup>();
    }

    public class UserGroup
    {
        public int[] Color { get; set; } = Array.Empty<int>();
        public UInt64[] Users { get; set; } = Array.Empty<UInt64>();
        public string GroupImage { get; set; } = string.Empty;
    }
}