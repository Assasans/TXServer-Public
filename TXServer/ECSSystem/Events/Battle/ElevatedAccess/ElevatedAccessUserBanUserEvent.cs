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

			List<string> availableUnits = new() { "MINUTE", "HOUR", "DAY", "WARN" };
			if (!availableUnits.Contains(Regex.Replace(Type, "[0-9]", "").Replace("S", "")) && !String.IsNullOrEmpty(Type))
			{
				string errorMessage = $"Private message: Invalid/No unit found in paramter '{Type}'";
				sendErrorMessage(player, battle, errorMessage);
				return;
			}

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
				string durationIfNeeded = "";
				string unit = string.Empty;
				string punishment = string.Empty;
				List<string> punishments = new();

				for (int i=0; i< Type.Length; i++)
                {
					if (Char.IsDigit(Type[i]))
						duration += Type[i];
                }
				if (string.IsNullOrEmpty(duration))
					duration = "0";

				string chatLanguage = "ru";
				if (chatLanguage == "en")
                {
					punishments.AddRange(new List<string>() { "warned", "chat banned" });

					unit = Regex.Replace(Type, "[0-9]", "").Replace("S", "").ToLower();
					if (Convert.ToInt32(duration) > 1)
						unit += "s";
				}
				else
                {
					punishments.AddRange(new List<string>() { "предупрежден", "отключен от чата" });

					int intDuration = Convert.ToInt32(duration);
					List<string> dictUnit = new();
					if (Type.Replace("S", "") == "MINUTE")
                    {
						dictUnit.Add("минуту");
						dictUnit.Add("минуты");
						dictUnit.Add("минут");
                    }
					else if (Type.Replace("S", "") == "HOUR")
					{
						dictUnit.Add("час");
						dictUnit.Add("часа");
						dictUnit.Add("часов");
					}

					if (dictUnit.Any())
                    {
						unit = intDuration switch
						{
							1 => dictUnit[0],
							int n when n >= 2 && n <= 4 => dictUnit[1],
							_ => dictUnit[2],
						};
					}
					else
                    {
                        unit = intDuration switch
                        {
                            1 => "сутки",
                            _ => "суток",
                        };
                    }
				}
				punishment = Regex.Replace(Type, "[0-9]", "").Replace("S", "") switch
				{
					"WARN" => punishments[0],
					_ => punishments[1],
				};

				if (chatLanguage == "en")
				{
					if (Type != "WARN")
					{
                        durationIfNeeded = Type switch
                        {
                            "FOREVER" or "" => " forever",
                            _ => $" for {duration} {unit}",
                        };
                    }
					message = $"{Uid} was {punishment}{durationIfNeeded}. Reason: {Reason}";
				}
				else
                {
					if (Type != "WARN")
					{
                        durationIfNeeded = Type switch
                        {
                            "FOREVER" or "" => " навсегда",
                            _ => $" на {duration} {unit}",
                        };
                    }
					message = $"{Uid} был {punishment}{durationIfNeeded}. Причина: {Reason}";
				}

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
				sendErrorMessage(player, battle, message);
            }
        }
		public string Type { get; set; }

		static void sendErrorMessage(Player player, Core.Battles.Battle battle, string message)
        {
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
}