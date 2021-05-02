using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-4669704207166218448L)]
	public class ExitBattleEvent : ECSEvent
	{
		public void Execute(Player player, Entity battleUser)
        {
			if (player.BattlePlayer != null)
				player.BattlePlayer.WaitingForExit = true;
			else
				player.Spectator.WaitingForExit = true;
        }
	}
}