using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.EntityTemplates.Chat;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Chat
{
    [SerialVersionUID(1446035600297L)]
    public class SendChatMessageEvent : ECSEvent
    {
        public void Execute(Player player, Entity chat)
        {
            Message = Message.Trim();
            if (!IsValidToSend()) return;

            Core.Battles.Battle battle = player.BattlePlayer?.Battle;

            if (ChatCommands.CheckForCommand(player, Message, out string commandReply))
            {
                ChatMessageReceivedEvent.SystemMessageTarget(commandReply, chat, player);
                return;
            }
            if (player.Data.Mod && ModCommands.CheckForCommand(player, Message, out string reply))
            {
                ChatMessageReceivedEvent.SystemMessageTarget(reply, chat, player);
                return;
            }

            if (player.IsMuted)
            {
                Punishment mute = player.Data.Punishments.First(p => p.Type is PunishmentType.Mute);
                string errorMsg = player.Data.CountryCode.ToLowerInvariant() switch
                {
                    "ru" => $"Вы были отключены от чата {(mute.IsPermanent ? "навсегда" : $"на {Math.Round(mute.Duration.TotalMinutes)} минут")}. {(mute.Reason != null ? $"Причина: {mute.Reason}" : "")}",
                    _ => $"You have been muted {(mute.IsPermanent ? "forever" : $"for {Math.Round(mute.Duration.TotalMinutes)} minute(s)")}. {(mute.Reason != null ? $"Reason: {mute.Reason}" : "")}"
                };
                ChatMessageReceivedEvent.SystemMessageTarget(errorMsg, chat, player);
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
                        if (!p.EntityList.Contains(chat))
                            p.ShareEntities(chat);
                        if (p != player)
                            p.SharePlayers(player);
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

        private bool IsValidToSend()
        {
            // todo: word blacklist
            if (Message[1..].StartsWith("runCommand")) return false;
            if (Message.Length is not (> 0 and <= 400)) return false;
            return true;
        }

        public string Message { get; set; }
    }
}
