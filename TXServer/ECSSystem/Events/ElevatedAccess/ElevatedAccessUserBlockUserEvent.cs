using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1503470140938L)]
	public class ElevatedAccessUserBlockUserEvent : ElevatedAccessUserBasePunishEvent, ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (!player.Data.Admin) return;

			Core.Battles.Battle battle = player.BattlePlayer.Battle;
			Player punishedPlayer = GetPunishedPlayer(battle, Uid);

			if (punishedPlayer == null)
			{
				string errorMsg = GetErrorMsg("playerNotFound", Uid, player);
				ChatMessageReceivedEvent.SystemMessageTarget(errorMsg, battle.GeneralBattleChatEntity, player);
				return;
			}
			if (punishedPlayer.User.GetComponent<UserAdminComponent>() == null)
			{
				string errorMsg = GetErrorMsg("adminPlayer", Uid, player);
                ChatMessageReceivedEvent.SystemMessageTarget(errorMsg, battle.GeneralBattleChatEntity, player);
				return;
			}

			foreach (BattleTankPlayer battleLobbyPlayer in battle.PlayersInMap)
            {
				string playerLanguage = battleLobbyPlayer.User.GetComponent<UserCountryComponent>().CountryCode;
				string message = playerLanguage switch
				{
					"RU" => $"{Uid} был заблокирован. Причина: {TranslateReason(Reason, playerLanguage)}",
					_ => $"{Uid} was banned. Reason: {TranslateReason(Reason, playerLanguage)}",
				};
                ChatMessageReceivedEvent.SystemMessageTarget(message, battle.GeneralBattleChatEntity, battleLobbyPlayer.Player);
				// todo: ban account
			}
		}
	}
}
