using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1511430732717L)]
	public class ElevatedAccessUserChangeEnergyEvent : ECSEvent
	{
        [Obsolete("Command is obsolete because energy has been removed")]
        public void Execute(Player player, Entity entity)
        {
            if (!player.Data.IsAdmin) return;

            ChatMessageReceivedEvent.SystemMessageTarget("Error: obsolete command, energy has been removed", player);
        }

		public int Count { get; set; }
	}
}
