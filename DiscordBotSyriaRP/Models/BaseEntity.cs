using DiscordBotSyriaRP.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DiscordBotSyriaRP.Models
{
    public class BaseEntity : IAuditable
    {
        [Key]
        public Guid Id { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
    }
}