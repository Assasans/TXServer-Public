using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.OldChatCommands
{
    [SerialVersionUID(636469074545384579L)]
    public class InviteUserToChatEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity1, Entity entity2)
        {
            if (!player.Data.Admin || !player.Data.Mod) return;

            Player goalPlayer = Server.Instance.FindPlayerByUsername(UserUid);
            if (goalPlayer is null)
            {
                ChatMessageReceivedEvent.SystemMessageTarget($"Error, couldn't find user '{UserUid}'", player);
                return;
            }
            if (goalPlayer == player)
            {
                ChatMessageReceivedEvent.SystemMessageTarget($"Error, you can't do this with yourself", player);
                return;
            }


            Entity chat = new Entity(new TemplateAccessor(new PersonalChatTemplate(), "chat"),
                new ChatComponent(),
                new ChatParticipantsComponent(player.User, goalPlayer?.User));

            player.SharePlayers(goalPlayer);
            player.ShareEntities(chat);
            player.User.ChangeComponent<PersonalChatOwnerComponent>(component => component.ChatsIs.Add(chat));
        }

        public string UserUid { get; set; }
    }
}
