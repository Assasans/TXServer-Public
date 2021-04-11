using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1499172594697L)]
	public class SwitchTeamEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			Core.Battles.Battle battle = player.BattlePlayer.Battle;
			
			if (battle.Params.BattleMode == BattleMode.DM)
				return;

			var handler = (Core.Battles.Battle.TeamBattleHandler)battle.ModeHandler;
			BattleView view = handler.BattleViewFor(player.BattlePlayer);

			player.User.RemoveComponent<TeamColorComponent>();
			player.User.AddComponent(new TeamColorComponent(view.EnemyTeamColor));

			view.AllyTeamPlayers.Remove(player.BattlePlayer);
			player.BattlePlayer = new BattlePlayer(battle, player, view.EnemyTeamEntity, false);
			view.EnemyTeamPlayers.Add(player.BattlePlayer);
		}
	}
}