namespace DiscordBotSyriaRP.Models
{
    public class User : BaseEntity
    {
        public UInt64 DiscordID { get; set; }
        public string DisordTag { get; set; } = string.Empty;
        public string AvatarLink { get; set; } = string.Empty;
    }
}