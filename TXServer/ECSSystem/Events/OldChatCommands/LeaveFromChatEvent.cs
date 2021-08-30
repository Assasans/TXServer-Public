using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.OldChatCommands
{
    [SerialVersionUID(636469076167979149L)]
    public class LeaveFromChatEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity1, Entity entity2)
        {
            if (!player.Data.IsAdmin || !player.Data.IsModerator) return;

            bool leftChat = false;
            foreach (Entity chat in player.EntityList.Where(c =>
                c.TemplateAccessor.Template.GetType() == typeof(PersonalChatTemplate)))
            {
                leftChat = true;
                player.UnshareEntities(chat);
                player.User.ChangeComponent<PersonalChatOwnerComponent>(component =>
                {
                    if (component.ChatsIs.Contains(chat))
                        component.ChatsIs.Remove(chat);
                });
            }

            if (!leftChat)
                ChatMessageReceivedEvent.SystemMessageTarget("Didn't find any private chats to leave", player);
        }
    }
}
