using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(1399558738794728790)]
    public class UserReadyToBattleComponent : Component
    {
		public void OnAttached(Player player, Entity battleUser)
		{
			if (!player.BattlePlayer.IsSpectator)
			    player.BattlePlayer.MatchPlayer.TankState = TankState.Spawn;
		}
	}
}