﻿using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;
using System;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1497356545125L)]
	public class StartBattleEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			foreach (Core.Battles.Battle battle in ServerConnection.BattlePool)
            {
				if (player == battle.Owner)
                {
					battle.BattleState = Core.Battles.BattleState.Starting;
                }
            }
		}
	}
}