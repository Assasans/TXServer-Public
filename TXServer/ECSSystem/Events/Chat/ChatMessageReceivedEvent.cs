using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Chat
{
    [SerialVersionUID(1450950140104L)]
	public class ChatMessageReceivedEvent : ECSEvent
	{
        private static ChatMessageReceivedEvent CreateMessage(string message, Player receiver, Player sender = null)
        {
            if (sender is not null && receiver.Data.BlockedPlayerIds.Contains(sender.User.EntityId))
                return CreateBlockedMessage(receiver, sender);

            return new ChatMessageReceivedEvent
            {
                Message = message,
                SystemMessage = sender is null,
                UserId = sender?.User.EntityId ?? 0,
                UserUid = sender?.Data.Username ?? "System",
                UserAvatarId = sender?.User.GetComponent<UserAvatarComponent>().Id ?? ""
            };
        }
        private static ChatMessageReceivedEvent CreateBlockedMessage(Player receiver, Player sender)
        {
            return new()
            {
                Message = receiver.Data.CountryCode is "ru" ? "[заблокированное сообщение]" : "[Blocked message]",
                SystemMessage = false,
                UserId = sender.User.EntityId,
                UserUid = receiver.Data.CountryCode is "ru" ? "[Заблокирован]": "[Blocked]",
                UserAvatarId = ""
            };
        }

        /// <summary>Send message to multiple players.</summary>
        /// <param name="message">Content of the message to be sent</param>
        /// <param name="chat">The chat in which the message should show up</param>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="receiver">Targeted players</param>
        public static void MessageTargets(string message, Entity chat, Player sender, IEnumerable<Player> receiver)
        {
            foreach (Player messageReceiver in receiver)
                messageReceiver.SendEvent(CreateMessage(message, messageReceiver, sender), chat);
        }
        public static void MessageTargets(string message, Entity chat, Player sender, IEnumerable<BattleTankPlayer> receiver)
            => MessageTargets(message, chat, sender, receiver.Select(p => p.Player));

        /// <summary>Send system message to a player in a specific chat.</summary>
        /// <param name="message">Content of the message to be sent</param>
        /// <param name="chat">The chat in which the system message should show up</param>
        /// <param name="target">Targeted player</param>
        public static void SystemMessageTarget(string message, Entity chat, Player target) =>
            target.SendEvent(CreateMessage(message, target), chat);
        /// <summary>Send system message to a player & let the system choose the best chat target.</summary>
        /// <param name="message">Content of the message to be sent</param>
        /// <param name="target">Targeted player</param>
        public static void SystemMessageTarget(string message, Player target)
        {
            Entity chat;
            if (target.IsInMatch)
                chat = target.BattlePlayer.Battle.GeneralBattleChatEntity;
            else if (target.IsInBattle)
                chat = target.BattlePlayer.Battle.BattleLobbyChatEntity;
            else
                chat = Chats.GlobalItems.Ru;

            target.SendEvent(CreateMessage(message, target), chat);
        }
        /// <summary>Send system message to multiple players & multiple chats.</summary>
        public static void SystemMessageTarget(string message, Dictionary<IEnumerable<Player>, Entity> targets)
        {
            foreach ((IEnumerable<Player> targetedPlayer, Entity value) in targets)
            foreach (Player target in targetedPlayer)
                SystemMessageTarget(message, value, target);
        }

        /// <summary>Send system message to multiple players except one specific & multiple chats.</summary>
        public static void SystemMessageOtherPlayers(string message, Dictionary<IEnumerable<Player>, Entity> targets,
            Player ignoredPlayer)
        {
            foreach ((IEnumerable<Player> targetedPlayer, Entity value) in targets)
            foreach (Player target in targetedPlayer.Where(p => p != ignoredPlayer))
                SystemMessageTarget(message, value, target);
        }
        /// <summary>Send system message to multiple players except one specific.</summary>
        public static void SystemMessageOtherPlayers(string message, IEnumerable<Player> targets, Entity chat,
            Player ignoredPlayer)
        {
            foreach (Player target in targets.Where(p => p != ignoredPlayer))
                SystemMessageTarget(message, chat, target);
        }

        /// <summary>Send punishment message to multiple players except one specific.</summary>
        public static void PunishMessageOtherPlayers(Punishment punishment, IEnumerable<Player> targets, Entity chat,
            Player ignoredPlayer)
        {
            foreach (Player target in targets.Where(p => p != ignoredPlayer))
                SystemMessageTarget(punishment.CreatePunishmentMsg(target), chat, target);

        }

		public string Message { get; set; }

		public bool SystemMessage { get; set; }

		public string UserUid { get; set; }

		public long UserId { get; set; }

		public string UserAvatarId { get; set; }
	}
}
