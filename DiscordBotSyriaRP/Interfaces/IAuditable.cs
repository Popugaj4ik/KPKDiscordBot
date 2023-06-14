namespace DiscordBotSyriaRP.Interfaces
{
    public interface IAuditable
    {
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
    }
}