using System.Numerics;
using TXServer.Core;
using TXServer.Core.Battles.BattleWeapons;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(4186891190183470299L)]
	public class ShaftAimingWorkingStateComponent : Component
    {
        public void OnAttached(Player player, Entity weapon) =>
            ((Shaft) player.BattlePlayer.MatchPlayer.BattleWeapon).StartAiming();

        public void OnRemove(Player player, Entity weapon) =>
            ((Shaft) player.BattlePlayer.MatchPlayer.BattleWeapon).StopAiming();

        public float InitialEnergy { get; set; }
        public float ExhaustedEnergy { get; set; }
        public float VerticalAngle { get; set; }
        public Vector3 WorkingDirection { get; set; }
        public float VerticalSpeed { get; set; }
        public int VerticalElevationDir { get; set; }
        public bool IsActive { get; set; }
        public int ClientTime { get; set; }
	}
}
