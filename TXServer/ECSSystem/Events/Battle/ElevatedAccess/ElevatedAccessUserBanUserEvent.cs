using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1503470104769L)]
	public class ElevatedAccessUserBanUserEvent : ElevatedAccessUserBasePunishEvent, ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = ServerConnection.BattlePool.Single(b => b.MatchPlayers.Contains(player.BattleLobbyPlayer));
			Player punishedPlayer = null;

			string uid = "";
			foreach (BattleLobbyPlayer battleLobbyPlayer in battle.MatchPlayers)
            {
				try { uid = battleLobbyPlayer.User.GetComponent<UserUidComponent>().Uid; }
				catch { }

				if (uid == Uid)
					punishedPlayer = battleLobbyPlayer.Player;
					break;
            }

			string message = string.Empty;
			if (punishedPlayer != null) {
				string duration = string.Empty;
				string unit = string.Empty;
				string punishment = string.Empty;
				Dictionary<string, string> punishmentsEN = new Dictionary<string, string>
				{
					{ "WARN", "warned" },
					{ "MINUTE", "chat banned" },
					{ "HOUR", "chat banned" },
					{ "DAY", "chat banned" }
				};
				Dictionary<string, string> unitsEN = new Dictionary<string, string>
				{
					{ "MINUTE", "minute" },
					{ "HOUR", "hour" },
					{ "DAY", "day" }
				};

				for (int i=0; i< Type.Length; i++)
                {
					if (Char.IsDigit(Type[i]))
						duration += Type[i];
                }
				if (string.IsNullOrEmpty(duration))
					duration = "0";

				string chatLanguage = "en";
				if (chatLanguage == "en")
                {
					punishment = punishmentsEN[Regex.Replace(Type, "[0-9]", "").Replace("S", "")];

					unit = unitsEN[Regex.Replace(Type, "[0-9]", "").Replace("S", "")];
					if (Convert.ToInt32(duration) > 1)
						unit += "s";

					message = $"{Uid} was {punishment} for {duration} {unit}. Reason: {Reason}";
				}

				if (string.IsNullOrEmpty(message))
					return;

				battle.MatchPlayers.Select(x => x.Player).SendEvent(new ChatMessageReceivedEvent
				{
					Message = message,
					SystemMessage = true,
					UserId = player.User.EntityId,
					UserUid = player.User.GetComponent<UserUidComponent>().Uid,
					UserAvatarId = player.User.GetComponent<UserAvatarComponent>().Id
				}, battle.GeneralBattleChatEntity);
			}
			else
            {
				message = $"Private message: {Uid} wasn't found in this battle";
				player.SendEvent(new ChatMessageReceivedEvent
				{
					Message = message,
					SystemMessage = true,
					UserId = player.User.EntityId,
					UserUid = player.User.GetComponent<UserUidComponent>().Uid,
					UserAvatarId = player.User.GetComponent<UserAvatarComponent>().Id
				}, battle.GeneralBattleChatEntity);
            }
        }
		public string Type { get; set; }
	}
}