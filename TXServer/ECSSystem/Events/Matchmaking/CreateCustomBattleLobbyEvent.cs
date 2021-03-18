using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1496750075382L)]
	public class CreateCustomBattleLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			var battle = new Core.Battles.Battle(battleParams: Params, isMatchMaking: false, owner: player);
			ServerConnection.BattlePool.Add(battle);
			battle.AddPlayer(player);
		}

		public ClientBattleParams Params { get; set; }
	}
}