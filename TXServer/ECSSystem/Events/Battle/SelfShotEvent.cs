using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(5440037691022467911L)]
    public class SelfShotEvent : ECSEvent
    {
        [OptionalMapped]
        public Vector3 ShotDirection { get; set; }

        public int ShotId { get; set; }

        public int ClientTime { get; set; }
    }
}