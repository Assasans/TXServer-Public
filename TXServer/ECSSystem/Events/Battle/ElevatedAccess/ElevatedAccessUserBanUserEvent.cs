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
			Core.Battles.Battle battle = GetBattle(player);
			Player punishedPlayer = GetPunishedPlayer(battle, Uid);

			List<string> availableUnits = new() { "minute", "hour", "day", "warn" };
			if (!availableUnits.Contains(Regex.Replace(Type, "[0-9]", "").Replace("S", "").ToLower()) && !String.IsNullOrEmpty(Type))
			{
				string errorMessage = GetErrorMsg("unknownType", Type, player);
                SendMessage(player, errorMessage, battle);
				return;
			}
			if (punishedPlayer == null)
            {
				string errorMsg = GetErrorMsg("playerNotFound", Uid, player);
				SendMessage(player, errorMsg, battle);
				return;
			}
            if (punishedPlayer.User.GetComponent<UserAdminComponent>() == null)
            {
                string errorMsg = GetErrorMsg("adminPlayer", Uid, player);
                SendMessage(player, errorMsg, battle);
                return;
            }


            string duration = string.Empty;
            for (int i = 0; i < Type.Length; i++)
            {
                if (Char.IsDigit(Type[i]))
                    duration += Type[i];
            }
            if (string.IsNullOrEmpty(duration) && Type != "WARN")
                duration = "1";

            Dictionary<string, string> languages = new() { { "EN", "" }, { "RU", "" } };
            foreach (KeyValuePair<string, string> translation in languages.ToArray())
            {
                string unit;
                string durationIfNeeded = "";
                List<string> punishments = new();

                switch (translation.Key)
                {
                    case "RU":
                        punishments.AddRange(new List<string>() { "предупрежден", "отключен от чата" });
                        if (Type != "WARN")
                        {
                            int intDuration = Convert.ToInt32(duration);
                            List<string> minuteTranslations = new() { "минуту", "минуты", "минут" };
                            List<string> hourTranslations = new() { "час", "часа", "часов" };
                            List<string> dictUnit = Type.Replace("S", "") switch
                            {
                                "MINUTE" => minuteTranslations,
                                "HOUR" => hourTranslations,
                                _ => new()
                            };
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
                            durationIfNeeded = Type switch
                            {
                                "FOREVER" or "" => " навсегда",
                                _ => $" на {duration} {unit}",
                            };
                        }
                        break;
                    default:
                        punishments.AddRange(new List<string>() { "warned", "chat banned" });
                        if (Type != "WARN")
                        {
                            unit = Regex.Replace(Type, "[0-9]", "").Replace("S", "").ToLower();
                            if (Convert.ToInt32(duration) > 1)
                                unit += "s";
                            durationIfNeeded = Type switch
                            {
                                "FOREVER" or "" => " forever",
                                _ => $" for {duration} {unit}",
                            };
                        }
                        break;
                }
                string punishment = Regex.Replace(Type, "[0-9]", "").Replace("S", "").ToLower() switch
                {
                    "warn" => punishments[0],
                    _ => punishments[1],
                };
                languages[translation.Key] = translation.Key switch
                {
                    "RU" => $"{Uid} был {punishment}{durationIfNeeded}. Причина: {TranslateReason(Reason, translation.Key)}",
                    _ => $"{Uid} was {punishment}{durationIfNeeded}. Reason: {TranslateReason(Reason, translation.Key)}",
                };
            }
            
            foreach (BattlePlayer battleLobbyPlayer in battle.MatchPlayers)
            {
                SendMessage(battleLobbyPlayer.Player, languages[battleLobbyPlayer.User.GetComponent<UserCountryComponent>().CountryCode], battle);
            }
            // todo: ban user from chat
        }
		public string Type { get; set; }
	}
}