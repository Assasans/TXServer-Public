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
            Player goalPlayer = Server.Instance.Connection.Pool.FirstOrDefault(p => p.UniqueId == UserUid.Replace("Survivor ", "").Replace("Deserter ", ""));
            if (goalPlayer is null) return;

            Entity chat = null;
            List<Entity> chatParticipants = new() { player.User, goalPlayer.User };
            foreach (Entity participant in chatParticipants)
            {
                Entity otherUser = chatParticipants.Single(user => user.EntityId != participant.EntityId);
                chat = participant.GetComponent<PersonalChatOwnerComponent>().ChatsIs.FirstOrDefault(personalChat => personalChat.GetComponent<ChatParticipantsComponent>().Users.Contains(otherUser));
                if (chat != null) break;
            }
            
            if (chat == null)
            {
                chat = new Entity(new TemplateAccessor(new PersonalChatTemplate(), "chat"),
                    new ChatComponent(),
                    new ChatParticipantsComponent(user, goalPlayer.User));

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
