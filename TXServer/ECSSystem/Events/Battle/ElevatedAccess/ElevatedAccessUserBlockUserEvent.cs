using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1503470140938L)]
	public class ElevatedAccessUserBlockUserEvent : ElevatedAccessUserBasePunishEvent, ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			Core.Battles.Battle battle = player.BattlePlayer.Battle;
			Player punishedPlayer = GetPunishedPlayer(battle, Uid);

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

			foreach (BattlePlayer battleLobbyPlayer in battle.MatchPlayers)
            {
				string playerLanguage = battleLobbyPlayer.User.GetComponent<UserCountryComponent>().CountryCode;
				string message = playerLanguage switch
				{
					"RU" => $"{Uid} был заблокирован. Причина: {TranslateReason(Reason, playerLanguage)}",
					_ => $"{Uid} was banned. Reason: {TranslateReason(Reason, playerLanguage)}",
				};
				SendMessage(battleLobbyPlayer.Player, message, battle);
				// todo: ban account
			}
		}
	}
}