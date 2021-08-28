using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.Types.Battle;

namespace TXServer.ECSSystem.Events.Lobby
{
	[SerialVersionUID(1497614958932L)]
	public class UpdateBattleParamsEvent : ECSEvent
	{
		public void Execute(Player player, Entity session)
        {
            player.BattlePlayer.Battle.ExtendedBattleMode =
                (ExtendedBattleMode) (int) player.BattlePlayer.Battle.Params.BattleMode;
			player.BattlePlayer.Battle.UpdateParams(Params);
		}

		public ClientBattleParams Params { get; set; }
	}
}
