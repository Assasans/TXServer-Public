﻿using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1503470140938L)]
	public class ElevatedAccessUserBlockUserEvent : ElevatedAccessUserBasePunishEvent, ECSEvent
	{
        public void Execute(Player player, Entity entity)
        {
            if (!player.Data.IsAdmin) return;

            ChatMessageReceivedEvent.SystemMessageTarget(ModCommands.Mute(player, new []{Uid, Reason}), player);
        }
    }
}
