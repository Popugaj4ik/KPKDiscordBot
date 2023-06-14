namespace DiscordBotSyriaRP.Constants
{
    public class GlobalConstants
    {
        public const string FlagStart = "--";
        public const string Anon = "anon";
        public const string Msg = "msg";
        public const string Avatar = "Avatar";
        public const string AddGroup = "GroupAdd";
        public const string ListGroups = "ListGroups";
        public const string DeleteGroup = "GroupDelete";
        public const string EditGroup = "GroupEdit";
        public const string AddGroupMember = "GroupAddMember";
        public const string RemoveGroupMember = "GroupRemoveMember";
        public const string FlagDiscordUser = $"{FlagStart}discorduser";
        public const string FlagDiscordRole = $"{FlagStart}discordrole";
        public const string FlagAsPerson = $"{FlagStart}asperson";
        public const string FlagPersonAvatar = $"{FlagStart}aspersonavatar";
        public const string MessageFrom = $"От:";
        public const string MessageTo = $"Кому:";
        public const string MessageToAll = $"Всем";
        public const string MessageContent = $"Сообщение:";
        public const string Anonim = $"Аноним";
        public const string Deanon = $"Деанон";
        public const string LogFile = "Log.txt";

        public const string CallModalCommand = "component";

        public static string GetRole(string role) => $"<@&{role}>";

        public static string GetUser(string user) => $"<@{user}>";
    }
}