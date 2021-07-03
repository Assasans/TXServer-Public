using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hit
{
    [SerialVersionUID(1430210549752L)]
	public class SelfUpdateStreamHitEvent : BaseUpdateStreamHitEvent, ISelfEvent
	{
        public void Execute(Player player, Entity weapon) => SelfEvent.Execute(this, player, weapon);
        public IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteUpdateStreamHitEvent>();
	}
}
