using System;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.OldChatCommands
{
    [SerialVersionUID(636469075744282523L)]
    public class KickUserFromChatEvent : ECSEvent
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

            Player[] chatPlayers = { goalPlayer, player };
            // ReSharper disable once PossibleNullReferenceException
            foreach (Player p in chatPlayers)
            {
                foreach (Entity chat in p.User.GetComponent<PersonalChatOwnerComponent>().ChatsIs.ToList()
                    .Where(c => c.GetComponent<ChatParticipantsComponent>().Users
                        .Contains(chatPlayers.Single(pl => p != pl).User)))
                {
                    foreach (Player pl in new []{goalPlayer, player})
                    {
                        pl.UnshareEntities(chat);
                        pl.User.ChangeComponent<PersonalChatOwnerComponent>(component =>
                        {
                            if (component.ChatsIs.Contains(chat))
                                component.ChatsIs.Remove(chat);
                        });
                    }

                    return;
                }
            }

            ChatMessageReceivedEvent.SystemMessageTarget(
                    $"Didn't find a chat between '{goalPlayer.Data.Username}' and you", player);
        }

        public string UserUid { get; set; }
    }
}
