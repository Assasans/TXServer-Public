using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Smoky
{
    [SerialVersionUID(-4247034853035810941L)]
    public class CriticalDamageEvent : ECSEvent
    {
        public CriticalDamageEvent(Entity target, Vector3 localPosition)
        {
            Target = target;
            LocalPosition = localPosition;
        }

        public Entity Target { get; set; }
        public Vector3 LocalPosition { get; set; }
    }
}
