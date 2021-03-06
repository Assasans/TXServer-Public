using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1514369648926L)]
	public class ElevatedAccessUserAddScoreEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (!player.Data.Admin) return;

			player.BattlePlayer?.MatchPlayer.UpdateStatistics(additiveScore:Count, 0, 0, 0, null);
		}
		public int Count { get; set; }
	}
}
