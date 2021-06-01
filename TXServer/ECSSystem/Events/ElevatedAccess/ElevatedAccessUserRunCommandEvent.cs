﻿using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1503493668447L)]
    public class ElevatedAccessUserRunCommandEvent : ECSEvent
    {
        public void Execute(Player player, Entity session)
        {
            if (!player.Data.Admin) return;

            if (AdminCommands.CheckForCommand(Command, player, out string cmdReply))
                ChatMessageReceivedEvent.SystemMessageTarget(cmdReply, player);
        }

        public string Command { get; set; }
    }
}
