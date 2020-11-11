using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(196833391289212110L)]
	public class SelfSplashHitEvent : SelfHitEvent
	{
		[OptionalMapped]
		public List<HitTarget> SplashTargets { get; set; }
	}
}