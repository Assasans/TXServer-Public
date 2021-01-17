﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1496750075382L)]
	public class CreateCustomBattleLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			ServerConnection.BattlePool.Add(new Core.Battles.Battle(battleParams:Params, isMatchMaking:false, owner:player));
			int index = 0;
			foreach (Core.Battles.Battle battle in ServerConnection.BattlePool)
            {
				if (battle.Owner == player)
                {
					ServerConnection.BattlePool[index].AddPlayer(player);
					break;
				}
				index++;
            }
		}

		public ClientBattleParams Params { get; set; }
	}
}