using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    public abstract class HitEvent : ECSEvent
    {
        public int ShotId { get; set; }

        [OptionalMapped]
        public StaticHit StaticHit { get; set; }

        [OptionalMapped]
        public List<HitTarget> Targets { get; set; }

        public int ClientTime { get; set; }
    }
}
