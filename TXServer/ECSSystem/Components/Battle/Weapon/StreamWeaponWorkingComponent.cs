using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(971549724137995758L)]
	public class StreamWeaponWorkingComponent : Component
	{
        public void OnAttached(Player player, Entity battleUser) =>
            player.BattlePlayer.MatchPlayer.TryDeactivateInvisibility();

        public void OnRemove(Player player, Entity battleUser) =>
            player.BattlePlayer.MatchPlayer.StreamHitLengths.Clear();

        public int Time { get; set; }
	}
}
