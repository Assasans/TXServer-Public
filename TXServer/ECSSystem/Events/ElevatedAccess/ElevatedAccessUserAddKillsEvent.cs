using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1515485268330L)]
	public class ElevatedAccessUserAddKillsEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
        {
            if (!player.IsInMatch || !player.Data.Admin) return;
			var battlePlayer = (BattleTankPlayer)player.BattlePlayer;

			battlePlayer.MatchPlayer.UpdateStatistics(0, additiveKills:Count, 0, 0, null);
			if (battlePlayer.Battle.ModeHandler is TDMHandler)
				battlePlayer.Battle.UpdateScore(battlePlayer.Team, Count);
		}
		public int Count { get; set; }
	}
}
