using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Lobby
{
	[SerialVersionUID(1547630520757L)]
	public class OpenCustomLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			// 1000 Blue-Crystals standard, 0 with premium
            player.Data.Crystals -= player.Data.IsPremium ? 0 : 1000;

			((CustomBattleHandler)player.BattlePlayer.Battle.TypeHandler).OpenBattle();
		}
	}
}
