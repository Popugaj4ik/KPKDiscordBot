using Discord.Interactions;
using DiscordBotSyriaRP.Constants;

namespace DiscordBotSyriaRP.Modals
{
    public class AnonModal : IModal
    {
        public string Title => "Отправка анонимного сообщения";

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalSendToUserLabel)]
        [ModalTextInput(ComponentsIDs.ModalSendToUserID, Discord.TextInputStyle.Short, placeholder: "ID пользователя или имя персонажа")]
        public string SendToUser { get; set; }

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalSendToRoleLabel)]
        [ModalTextInput(ComponentsIDs.ModalSendToRoleID, Discord.TextInputStyle.Short, placeholder: "ID роли")]
        public string SendToRole { get; set; }

        [RequiredInput]
        [InputLabel(ComponentsIDs.ModalMessageLabel)]
        [ModalTextInput(ComponentsIDs.ModalMessageID, Discord.TextInputStyle.Paragraph, placeholder: "Ваше сообщение")]
        public string Message { get; set; }
    }
}