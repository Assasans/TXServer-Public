using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1494934093730L)]
    public class DamageInfoEvent : ECSEvent
    {
        public DamageInfoEvent(float damage, Vector3 hitPoint = new(), bool backHit = false, bool healHit = false)
        {
            Damage = damage;
            HitPoint = hitPoint;
            BackHit = backHit;
            HealHit = healHit;
        }

        public float Damage { get; set; }
        public Vector3 HitPoint { get; set; }
        public bool BackHit { get; set; }
        public bool HealHit { get; set; }
    }
}
