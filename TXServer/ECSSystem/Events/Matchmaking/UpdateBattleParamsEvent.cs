using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1497614958932L)]
	public class UpdateBattleParamsEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			foreach (Core.Battles.Battle battle in ServerConnection.BattlePool)
            {
				if (battle.Owner == player)
                {
					battle.UpdateBattleParams(player, Params);
					break;
                }
            }
		}
		public ClientBattleParams Params { get; set; }
	}
}