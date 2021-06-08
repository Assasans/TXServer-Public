using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(635824352777145226L)]
    public class StreamHitCheckingComponent : Component
    {
        public StreamHitCheckingComponent(float lastSendToServerTime, float lastCheckTime, HitTarget lastSentTankHit,
            StaticHit lastSentStaticHit)
        {
            LastSendToServerTime = lastSendToServerTime;
            LastCheckTime = lastCheckTime;
            LastSentTankHit = lastSentTankHit;
            LastSentStaticHit = lastSentStaticHit;
        }

        public float LastSendToServerTime { get; set; }
        public float LastCheckTime { get; set; }
        public HitTarget LastSentTankHit { get; set; }
        public StaticHit LastSentStaticHit { get; set; }
    }
}
