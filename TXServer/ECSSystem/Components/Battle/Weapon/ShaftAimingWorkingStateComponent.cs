using System;
using System.Numerics;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Energy;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(4186891190183470299L)]
	public class ShaftAimingWorkingStateComponent : Component
    {
        public void OnAttached(Player player, Entity weapon) =>
            player.BattlePlayer.MatchPlayer.ShaftAimingBeginTime = DateTimeOffset.UtcNow;

        public void OnRemove(Player player, Entity weapon)
        {
            player.BattlePlayer.MatchPlayer.ShaftLastAimingDurationMs = (DateTimeOffset.UtcNow - (
                player.BattlePlayer.MatchPlayer.ShaftAimingBeginTime ?? DateTimeOffset.UtcNow)).TotalMilliseconds;
            player.BattlePlayer.MatchPlayer.ShaftAimingBeginTime = null;

            double energy = (double) (player.BattlePlayer.MatchPlayer.ShaftLastAimingDurationMs * 0.0002);
            weapon.ChangeComponent<WeaponEnergyComponent>(component => component.Energy = (float) (0.9 - energy));
        }


        public float InitialEnergy { get; set; }
        public float ExhaustedEnergy { get; set; }
        public float VerticalAngle { get; set; }
        public Vector3 WorkingDirection { get; set; }
        public float VerticalSpeed { get; set; }
        public int VerticalElevationDir { get; set; }
        public bool IsActive { get; set; }
	}
}
