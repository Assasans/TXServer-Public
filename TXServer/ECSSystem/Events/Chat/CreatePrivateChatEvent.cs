using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events.Chat
{
    [SerialVersionUID(636469080057216111L)]
    public class CreatePrivateChatEvent : ECSEvent
    {
        public void Execute(Player player, Entity user, Entity sourceChat)
        {
            Player goalPlayer =
                Server.Instance.FindPlayerByUsername(UserUid);
            if (goalPlayer is null)
            {
                ChatMessageReceivedEvent.SystemMessageTarget($"Error, couldn't find user '{UserUid}'", player);
                return;
            }

            Entity chat = null;
            List<Entity> chatParticipants = new() { player.User, goalPlayer?.User };
            foreach (Entity participant in chatParticipants)
            {
                Entity otherUser = chatParticipants.Single(u => u.EntityId != participant.EntityId);
                chat = participant.GetComponent<PersonalChatOwnerComponent>().ChatsIs.FirstOrDefault(personalChat =>
                    personalChat.GetComponent<ChatParticipantsComponent>().Users.Contains(otherUser));
                if (chat != null) break;
            }

            if (chat == null)
            {
                chat = new Entity(new TemplateAccessor(new PersonalChatTemplate(), "chat"),
                    new ChatComponent(),
                    new ChatParticipantsComponent(user, goalPlayer?.User));

                player.SharePlayers(goalPlayer);
            }
            else
            {
                player.User.ChangeComponent<PersonalChatOwnerComponent>(component => component.ChatsIs.Remove(chat));
                player.UnshareEntities(chat);
            }

            player.ShareEntities(chat);
            player.User.ChangeComponent<PersonalChatOwnerComponent>(component => component.ChatsIs.Add(chat));
        }

        public string UserUid { get; set; }
    }
}
