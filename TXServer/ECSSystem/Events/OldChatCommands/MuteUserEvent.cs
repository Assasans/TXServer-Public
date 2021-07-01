using System;
using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.OldChatCommands
{
    [SerialVersionUID(636469076339876375L)]
    public class MuteUserEvent : ECSEvent
    {
        [Obsolete("MuteUserEvent is deprecated & inaccurate, please use '!mute' as chat command instead.")]
        public void Execute(Player player, Entity entity1, Entity entity2)
        {
            if (!player.Data.Admin || !player.Data.Mod) return;

            string chatCmd = $"!mute {UserUid} forever";
            ModCommands.CheckForCommand(player, chatCmd, out string commandReply);
            ChatMessageReceivedEvent.SystemMessageTarget(commandReply, player);
        }

        public string UserUid { get; set; }
    }
}
