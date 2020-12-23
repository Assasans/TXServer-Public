using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-2203330189936241204L)]
	public class RemoteSplashHitEvent : RemoteHitEvent, IRemoteEvent
	{
		[OptionalMapped]
		public List<HitTarget> SplashTargets { get; set; }
	}
}