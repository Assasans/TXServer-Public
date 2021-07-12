using System;
using TXServer.Core;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1515481976775L)]
	public class ElevatedAccessUserWipeUserItemsEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (!player.IsInMatch || !player.Data.Admin) return;

            foreach (BattleModule module in player.BattlePlayer.MatchPlayer.Modules)
            {
                if (module.IsOnCooldown) return;
                module.CooldownEndTime = DateTimeOffset.UtcNow;
            }
        }
	}
}
