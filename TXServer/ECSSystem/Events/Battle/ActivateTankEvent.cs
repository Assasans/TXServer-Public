using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-5086569348607290080L)]
	public class ActivateTankEvent : ECSEvent
	{
		public void Execute(Player player, Entity tank)
        {
			player.BattlePlayer.MatchPlayer.CollisionsPhase = Phase;
        }

		public long Phase { get; set; }
	}
}