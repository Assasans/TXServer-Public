using TXServer.Core.Protocol;
using TXServer.ECSSystem.Events.Battle.Weapon.Shot;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hammer
{
    [SerialVersionUID(-1937089974629265090L)]
	public class SelfHammerShotEvent : SelfShotEvent
	{
		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHammerShotEvent>();
		public int RandomSeed { get; set; }
	}
}
