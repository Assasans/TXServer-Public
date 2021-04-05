﻿using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1439375251389)]
	public class IntroduceUserByUidEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			PlayerData data = player.Server.Database.FetchPlayerData(Uid);
			if (data == null) return; // Player#LogIn(Entity) will kick the player
			data.Player = player;
			player.Data = data;
			player.SendEvent(new PersonalPasscodeEvent(), entity);
		}

		public string Uid { get; set; }
	}
}
