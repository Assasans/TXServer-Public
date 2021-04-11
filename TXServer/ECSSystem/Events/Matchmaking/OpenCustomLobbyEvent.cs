using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Matchmaking
{
	[SerialVersionUID(1547630520757L)]
	public class OpenCustomLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			int price = 1000;  // 1000 Blue-Crystals standard price for opening custom battles
			if (player.IsPremium)
				price = 0;  // official premium pass feature: open custom battles for free
			player.Data.SetCrystals(player.Data.Crystals - price);

			((CustomBattleHandler)player.BattlePlayer.Battle.TypeHandler).OpenBattle();
		}
	}
}