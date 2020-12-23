using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    public class BaseUpdateStreamHitEvent : ECSEvent
	{
		[OptionalMapped]
		public StaticHit StaticHit { get; set; }

		[OptionalMapped]
		public HitTarget TankHit { get; set; }
	}
}