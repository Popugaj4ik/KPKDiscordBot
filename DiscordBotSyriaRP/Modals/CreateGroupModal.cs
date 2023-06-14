using Discord.Interactions;
using DiscordBotSyriaRP.Constants;

namespace DiscordBotSyriaRP.Modals
{
    public class CreateGroupModal : IModal
    {
        public string Title => "Создать группу";

        [RequiredInput]
        [InputLabel(ComponentsIDs.ModalGroupIDLabel)]
        [ModalTextInput(ComponentsIDs.ModalGroupIDID, Discord.TextInputStyle.Short, placeholder: "Айди котоырй придумываете вы")]
        public string GroupID { get; set; }

        [RequiredInput]
        [InputLabel(ComponentsIDs.ModalLeaderDiscordIDLabel)]
        [ModalTextInput(ComponentsIDs.ModalLeaderDiscordIDID, Discord.TextInputStyle.Short, placeholder: "Discord ID человека, которого вы хотиет сделать лидером")]
        public UInt64 LeaderDiscordID { get; set; }

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalGroupNameLabel)]
        [ModalTextInput(ComponentsIDs.ModalGroupNameID, Discord.TextInputStyle.Short, placeholder: "Имя вашей группы")]
        public string GroupName { get; set; }

        [RequiredInput(false)]
        [InputLabel(ComponentsIDs.ModalGroupImageURLLabel)]
        [ModalTextInput(ComponentsIDs.ModalGroupImageURLID, Discord.TextInputStyle.Short, placeholder: "Ссылка на иконку группы")]
        public string GroupImageUrl { get; set; }

        [RequiredInput]
        [InputLabel(ComponentsIDs.ModalGroupColorLabel)]
        [ModalTextInput(ComponentsIDs.ModalGroupColorID, Discord.TextInputStyle.Short, placeholder: "R;G;B")]
        public string GroupColor { get; set; }
    }
}