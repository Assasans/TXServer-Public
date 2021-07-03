using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Shot
{
    public abstract class ShotEvent : ECSEvent
    {
        [OptionalMapped]
        public Vector3 ShotDirection { get; set; }

        public int ShotId { get; set; }

        public int ClientTime { get; set; }
    }
}
