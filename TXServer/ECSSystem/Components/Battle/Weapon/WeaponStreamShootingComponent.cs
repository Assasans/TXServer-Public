using System;
using TXServer.Core;
using TXServer.Core.Battles.BattleWeapons;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(6803807621463709653L)]
	public class WeaponStreamShootingComponent : Component
	{
        public void OnAttached(Player player, Entity weapon)
        {
            if (player.BattlePlayer.MatchPlayer.BattleWeapon.GetType() == typeof(Vulcan))
                ((Vulcan) player.BattlePlayer.MatchPlayer.BattleWeapon).TrySaveShootingStartTime();
        }

        [OptionalMapped] public DateTime StartShootingTime { get; set; }

        public int Time { get; set; }
	}
}
