using System;
using System.Collections.ObjectModel;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.EntityTemplates.Chat;
using TXServer.ECSSystem.Types.Punishments;

namespace TXServer.ECSSystem.Events.Chat
{
    [SerialVersionUID(1446035600297L)]
    public class SendChatMessageEvent : ECSEvent
    {
        public void Execute(Player player, Entity chat)
        {
            Core.Battles.Battle battle = player.BattlePlayer?.Battle;

            ChatMessageReceivedEvent evt = new()
            {
                Message = Message,
                SystemMessage = false,
                UserId = player.User.EntityId,
                UserUid = player.User.GetComponent<UserUidComponent>().Uid,
                UserAvatarId = player.User.GetComponent<UserAvatarComponent>().Id
            };

            string reply = ChatCommands.CheckForCommand(player, Message);
            if (!string.IsNullOrEmpty(reply))
            {
                evt.Message = reply;
                evt.SystemMessage = true;
                player.SendEvent(evt, chat);
                return;
            }

            ReadOnlyCollection<ChatMute> mutes = player.Data.GetChatMutes();
            if (mutes.Count > 0)
            {
                // TODO(Assasans): Show all mutes to player
                ChatMute mute = mutes[0];

                evt.Message = player.User.GetComponent<UserCountryComponent>().CountryCode.ToLowerInvariant() switch
                {
                    "ru" => $"Вы были отключены от чата {(mute.IsPermanent ? "навсегда" : $"на {((TimeSpan)mute.Duration).TotalMinutes} минут")}. Причина: {mute.Reason}",
                    _ => $"You have been muted {(mute.IsPermanent ? "forever" : $"for {((TimeSpan)mute.Duration).TotalMinutes} minutes")}. Reason: {mute.Reason}"
                };
                evt.SystemMessage = true;

                player.SendEvent(evt, chat);
                return;
            }

            switch (chat.TemplateAccessor.Template)
            {
                case GeneralChatTemplate _:
                    player.Server.Connection.Pool.SendEvent(evt, chat);
                    break;
                case PersonalChatTemplate _:
                    foreach (Player p in chat.GetComponent<ChatParticipantsComponent>().GetPlayers())
                    {
                        if (!p.EntityList.Contains(chat))
                        {
                            p.SharePlayers(player);
                            p.ShareEntities(chat);
                        }

                        p.SendEvent(evt, chat);
                    }
                    break;
                case SquadChatTemplate _:
                    player.SquadPlayer.Squad.Participants.SendEvent(evt, chat);
                    break;
                case BattleLobbyChatTemplate _:
                    battle?.JoinedTankPlayers.SendEvent(evt, chat);
                    break;
                case GeneralBattleChatTemplate _:
                    battle?.PlayersInMap.SendEvent(evt, chat);
                    break;
                case TeamBattleChatTemplate _:
                    battle?.MatchTankPlayers.Where(x => x.Team == player.BattlePlayer.Team).SendEvent(evt, chat);
                    break;
            }
        }

        public string Message { get; set; }
    }
}
