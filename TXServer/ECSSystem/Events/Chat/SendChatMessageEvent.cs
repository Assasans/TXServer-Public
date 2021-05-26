using System;
using System.Collections.Generic;
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

            string commandReply = ChatCommands.CheckForCommand(player, Message);
            if (!string.IsNullOrEmpty(commandReply))
                ChatMessageReceivedEvent.SystemMessageTarget(commandReply, chat, player);

            ReadOnlyCollection<ChatMute> mutes = player.Data.GetChatMutes();
            if (mutes.Count > 0)
            {
                // TODO(Assasans): Show all mutes to player
                ChatMute mute = mutes[0];

                string error = player.Data.CountryCode.ToLowerInvariant() switch
                {
                    "ru" => $"Вы были отключены от чата {(mute.IsPermanent ? "навсегда" : $"на {((TimeSpan)mute.Duration).TotalMinutes} минут")}. Причина: {mute.Reason}",
                    _ => $"You have been muted {(mute.IsPermanent ? "forever" : $"for {((TimeSpan)mute.Duration).TotalMinutes} minutes")}. Reason: {mute.Reason}"
                };

                ChatMessageReceivedEvent.SystemMessageTarget(error, chat, player);
                return;
            }

            switch (chat.TemplateAccessor.Template)
            {
                case GeneralChatTemplate _:
                    ChatMessageReceivedEvent.MessageTargets(Message, chat, player, player.Server.Connection.Pool);
                    break;
                case PersonalChatTemplate _:
                    IEnumerable<Player> chatParticipants = chat.GetComponent<ChatParticipantsComponent>().GetPlayers().ToList();
                    foreach (Player p in chatParticipants)
                    {
                        if (!p.EntityList.Contains(chat)) continue;
                        p.SharePlayers(player);
                        p.ShareEntities(chat);
                    }
                    ChatMessageReceivedEvent.MessageTargets(Message, chat, player, chatParticipants);
                    break;
                case SquadChatTemplate _:
                    ChatMessageReceivedEvent.MessageTargets(Message, chat, player,
                        player.SquadPlayer.Squad.Participants.Select(sp => sp.Player));
                    break;
                case BattleLobbyChatTemplate _:
                    ChatMessageReceivedEvent.MessageTargets(Message, chat, player, battle?.JoinedTankPlayers);
                    break;
                case GeneralBattleChatTemplate _:
                    ChatMessageReceivedEvent.MessageTargets(Message, chat, player,
                        battle?.PlayersInMap.Select(bp => bp.Player));
                    break;
                case TeamBattleChatTemplate _:
                    ChatMessageReceivedEvent.MessageTargets(Message, chat, player,
                        battle?.MatchTankPlayers.Where(x => x.Team == player.BattlePlayer.Team));
                    break;
            }
        }

        public string Message { get; set; }
    }
}
