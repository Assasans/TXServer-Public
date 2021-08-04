using System;
using System.Numerics;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

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
        }


        public float ExhaustedEnergy { get; set; }
        public float InitialEnergy { get; set; }
        public bool IsActive { get; set; }
        public float VerticalAngle { get; set; }
        public int VerticalElevationDir { get; set; }
        public float VerticalSpeed { get; set; }
        public Vector3 WorkingDirection { get; set; }
		public int ClientTime { get; set; }
	}
}
