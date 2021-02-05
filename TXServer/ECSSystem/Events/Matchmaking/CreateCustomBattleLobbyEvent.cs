using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;
using System.Linq;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1496750075382L)]
	public class CreateCustomBattleLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			ServerConnection.BattlePool.Add(new Core.Battles.Battle(battleParams:Params, isMatchMaking:false, owner:player));
			ServerConnection.BattlePool.ToList().Single(b => b.Owner == player).AddPlayer(player);
		}

		public ClientBattleParams Params { get; set; }
	}
}