using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(-1316093147997460626L)]
	public class PauseEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			//TODO: pause counter + kick user after given time
			if (!player.BattleLobbyPlayer.BattlePlayer.Paused)
				entity.AddComponent(new PauseComponent());
			else
				entity.RemoveComponent<PauseComponent>();
			player.BattleLobbyPlayer.BattlePlayer.Paused = !player.BattleLobbyPlayer.BattlePlayer.Paused;
		}
	}
}