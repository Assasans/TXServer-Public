using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.Chat
{
    [SerialVersionUID(1450950140104L)]
	public class ChatMessageReceivedEvent : ECSEvent
	{

        /// <summary>
        /// Send system message to a player in a specific chat.
        /// </summary>
        /// <param name="message">Content of the message to be sent</param>
        /// <param name="chat">The chat in which the system message should show up</param>
        /// /// <param name="target">Targeted player</param>
        public static void SystemMessageTarget(string message, Entity chat, Player target)
        {
            target.SendEvent(new ChatMessageReceivedEvent
            {
                Message = message,
                SystemMessage = true,
                UserId = 0,
                UserUid = "System",
                UserAvatarId = ""
            }, chat);
        }

        /// <summary>
        /// Send system message to a player & let the system choose the best chat target.
        /// </summary>
        /// <param name="message">Content of the message to be sent</param>
        /// /// <param name="target">Targeted player</param>
        public static void SystemMessageTarget(string message, Player target)
        {
            Entity chat;
            if (target.IsInMatch)
                chat = target.BattlePlayer.Battle.GeneralBattleChatEntity;
            else if (target.IsInBattle)
                chat = target.BattlePlayer.Battle.BattleLobbyChatEntity;
            else
                chat = Chats.GlobalItems.Ru;

            target.SendEvent(new ChatMessageReceivedEvent
            {
                Message = message,
                SystemMessage = true,
                UserId = 0,
                UserUid = "System",
                UserAvatarId = ""
            }, chat);
        }

        /// <summary>
        /// Send system message to multiple players & multiple chats.
        /// </summary>
        public static void SystemMessageTarget(string message, Dictionary<IEnumerable<Player>, Entity> targets, Player player)
        {
            foreach ((IEnumerable<Player> targetedPlayer, Entity value) in targets)
            foreach (Player target in targetedPlayer)
                SystemMessageTarget(message, value, target);
        }


        /// <summary>
        /// Send system message to multiple players except one specific & multiple chats.
        /// </summary>
        public static void SystemMessageOtherPlayers(string message, Dictionary<IEnumerable<Player>, Entity> targets,
            Player ignoredPlayer)
        {
            foreach ((IEnumerable<Player> targetedPlayer, Entity value) in targets)
            foreach (Player target in targetedPlayer.Where(p => p != ignoredPlayer))
                SystemMessageTarget(message, value, target);
        }

		public string Message { get; set; }

		public bool SystemMessage { get; set; }

		public string UserUid { get; set; }

		public long UserId { get; set; }

		public string UserAvatarId { get; set; }
	}
}
