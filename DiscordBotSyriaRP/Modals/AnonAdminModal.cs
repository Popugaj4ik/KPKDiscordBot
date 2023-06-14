using Discord.Interactions;
using DiscordBotSyriaRP.Constants;

namespace DiscordBotSyriaRP.Modals
{
    public class AnonAdminModal : IModal
    {
        public string Title => "Отправка анонимного сообщения администратора";

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalSendToUserLabel)]
        [ModalTextInput(ComponentsIDs.ModalSendToUserID, Discord.TextInputStyle.Short, placeholder: "ID пользователя или имя персонажа")]
        public string SendToUser { get; set; }

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalSendToRoleLabel)]
        [ModalTextInput(ComponentsIDs.ModalSendToRoleID, Discord.TextInputStyle.Short, placeholder: "ID роли")]
        public string SendToRole { get; set; }

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalAsPersonLabel)]
        [ModalTextInput(ComponentsIDs.ModalAsPersonID, Discord.TextInputStyle.Short, placeholder: "Имя персонажа")]
        public string AsPerson { get; set; }

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalAsPersonAvatarLabel)]
        [ModalTextInput(ComponentsIDs.ModalAsPersonAvatarID, Discord.TextInputStyle.Short, placeholder: "URL сылка на изображение")]
        public string AsPersonAvatar { get; set; }

        [RequiredInput]
        [InputLabel(ComponentsIDs.ModalMessageLabel)]
        [ModalTextInput(ComponentsIDs.ModalMessageID, Discord.TextInputStyle.Paragraph, placeholder: "Ваше сообщение")]
        public string Message { get; set; }
    }
}