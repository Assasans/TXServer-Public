using System;
using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.OldChatCommands
{
    [SerialVersionUID(636469076535361064L)]
    public class UnmuteUserEvent : ECSEvent
    {
        [Obsolete("UnmuteUserEvent is deprecated & inaccurate, please use '!unmute' as chat command instead.")]
        public void Execute(Player player, Entity entity1, Entity entity2)
        {
            if (!player.Data.IsAdmin || !player.Data.IsModerator) return;

            string chatCmd = $"!unmute {UserUid}";
            ModCommands.CheckForCommand(player, chatCmd, out string commandReply);
            ChatMessageReceivedEvent.SystemMessageTarget(commandReply, player);
        }

        public string UserUid { get; set; }
    }
}
