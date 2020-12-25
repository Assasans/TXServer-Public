using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-1937089974629265090L)]
	public class SelfHammerShotEvent : SelfShotEvent
	{
		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHammerShotEvent>();
		public int RandomSeed { get; set; }
	}
}