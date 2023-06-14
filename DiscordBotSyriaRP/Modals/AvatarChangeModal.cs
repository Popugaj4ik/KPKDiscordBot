using Discord.Interactions;
using DiscordBotSyriaRP.Constants;

namespace DiscordBotSyriaRP.Modals
{
    public class AvatarChangeModal : IModal
    {
        public string Title => "Поменять аватар сообщений";

        [RequiredInput]
        [InputLabel(ComponentsIDs.ModalNewAvatarLabel)]
        [ModalTextInput(ComponentsIDs.ModalNewAvatarID, Discord.TextInputStyle.Short, placeholder: "URL на вашу картинку")]
        public string NewAvatarURL { get; set; }
    }
}