using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	public class ElevatedAccessUserBasePunishEvent : ECSEvent
	{
		public static Core.Battles.Battle GetBattle(Player player)
        {
			return player.BattlePlayer.Battle;
		}
		public static Player GetPunishedPlayer(Core.Battles.Battle battle, string punishedPlayerUid)
        {
			string uid = "";
			foreach (BattlePlayer battleLobbyPlayer in battle.MatchPlayers)
			{
				try { uid = battleLobbyPlayer.User.GetComponent<UserUidComponent>().Uid; }
				catch { }

				if (uid == punishedPlayerUid)
					return battleLobbyPlayer.Player;
			}
			return null;
		}
		public static string GetErrorMsg(string error, string parameter, Player player)
		{
			string playerLanguage = player.User.GetComponent<UserCountryComponent>().CountryCode;
            return error switch
            {
                "playerNotFound" => playerLanguage switch
                {
                    "RU" => $"Личное сообщение: Игрок '{parameter}' не найден в этой битве",
                    _ => $"Private message: Player '{parameter}' wasn't found in this battle"
                },
                "unknownType" => playerLanguage switch
                {
                    "RU" => $"Личное сообщение: Неверная единица времени: '{parameter}'",
                    _ => $"Private message: Invalid/No unit found in parameter '{parameter}'",
                },
				"adminPlayer" => playerLanguage switch
                {
					"RU" => $"Личное сообщение: Не возможно, '{parameter}' является администратором",
					_ => $"Private message: Not possible, '{parameter}' is an admin",
				},
                _ => playerLanguage switch
                {
                    "DE" => "Private Nachricht: Unbekannter Fehler",
                    "RU" => "Личное сообщение: Неизвестная ошибка",
                    _ => "Private message: Unknown error",
                },
            };
        }
		public static string TranslateReason(string reason, string playerLanguage)
        {
			string toTranslateReason = reason;
			string englishReason = string.Empty;
			string translatedReason = string.Empty;
			Dictionary<string, string> ToEnglish = new()
			{
				{ "spam", "flood" },
				{ "флуд", "flood" },
				{ "politik", "politics" },
				{ "обсуждение политики", "politics" },
				{ "beleidigend", "insulting" },
				{ "оскорбления", "insulting" },
				{ "cheaten", "cheating" },
				{ "использование читов", "cheating" },
				{ "hacken", "hacking" },
			};
			Dictionary<string, string> ToRussian = new() {
				{ "flood", "флуд" },
				{ "politics", "обсуждение политики" },
				{ "insulting", "оскорбления" },
				{ "cheating", "использование читов" },
				{ "hacking", "использование читов" }
			};

			if (ToEnglish.TryGetValue(reason.ToLower(), out englishReason))
				toTranslateReason = englishReason;
			translatedReason = playerLanguage switch
			{
				"RU" => ToRussian.SingleOrDefault(t => t.Key == toTranslateReason.ToLower()).Value ?? toTranslateReason,
				_ => toTranslateReason,
			};
			return char.ToUpper(translatedReason[0]) + translatedReason[1..];
        }
		public static void SendMessage(Player player, string message, Core.Battles.Battle battle)
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
		
		public string Uid { get; set; }
		public string Reason { get; set; }
	}
}