using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1515485268330L)]
	public class ElevatedAccessUserAddKillsEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (player.BattlePlayer != null)
            {
				player.BattlePlayer.MatchPlayer.UpdateStatistics(0, additiveKills:Count, 0, 0, null);
				if (player.BattlePlayer.Battle.ModeHandler is TDMHandler)
					player.BattlePlayer.Battle.UpdateScore(player.BattlePlayer.Team, Count);
			}
		}
		public int Count { get; set; }
	}
}