using TXServer.Core;
using TXServer.Core.Battles.BattleWeapons;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
	[SerialVersionUID(-6843896944033144903L)]
	public class VulcanSlowDownComponent : Component
	{
        public void OnRemove(Player player, Entity weapon)
        {
            if (!IsAfterShooting) return;

            ((Vulcan) player.BattlePlayer.MatchPlayer.BattleWeapon).LastVulcanHeatTactTime = null;
            ((Vulcan) player.BattlePlayer.MatchPlayer.BattleWeapon).VulcanShootingStartTime = null;
        }

        public bool IsAfterShooting { get; set; }
		public int ClientTime { get; set; }
	}
}
