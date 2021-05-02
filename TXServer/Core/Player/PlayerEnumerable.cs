﻿using System;
using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.Core
{
    public static class PlayerEnumerable
    {
		public static void SharePlayers(this IEnumerable<Player> players, params Player[] toShare) => SharePlayers(players, (IEnumerable<Player>)toShare);
		public static void SharePlayers(this IEnumerable<Player> players, IEnumerable<Player> toShare)
		{
			foreach (Player player in players)
				player.SharePlayers(toShare);
		}

		public static void UnsharePlayers(this IEnumerable<Player> players, params Player[] toShare) => UnsharePlayers(players, (IEnumerable<Player>)toShare);
		public static void UnsharePlayers(this IEnumerable<Player> players, IEnumerable<Player> toShare)
		{
			foreach (Player player in players)
				player.UnsharePlayers(toShare);
		}

		[Obsolete("Use ShareEntities instead.")]
		public static void ShareEntity(this IEnumerable<Player> players, Entity entity)
		{
			foreach (Player player in players)
				player.ShareEntity(entity);
		}

		public static void ShareEntities(this IEnumerable<Player> players, params Entity[] entities) => ShareEntities(players, (IEnumerable<Entity>)entities);
		public static void ShareEntities(this IEnumerable<Player> players, IEnumerable<Entity> entities)
		{
			foreach (Player player in players)
				player.ShareEntities(entities);
		}

		[Obsolete("Use UnshareEntities instead.")]
		public static void UnshareEntity(this IEnumerable<Player> players, Entity entity)
		{
			foreach (Player player in players)
				player.UnshareEntity(entity);
		}

		public static void UnshareEntities(this IEnumerable<Player> players, params Entity[] entities) => UnshareEntities(players, (IEnumerable<Entity>)entities);
		public static void UnshareEntities(this IEnumerable<Player> players, IEnumerable<Entity> entities)
		{
			foreach (Player player in players)
				player.UnshareEntities(entities);
		}

		public static void SendEvent(this IEnumerable<Player> players, ECSEvent @event, params Entity[] entities)
		{
			foreach (Player player in players)
				player.SendEvent(@event, entities);
		}
	}
}
