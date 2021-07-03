using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Railgun
{
    [SerialVersionUID(4963057750170414217L)]
    public class SelfRailgunChargingShotEvent : BaseRailgunChargingShotEvent, ISelfEvent
    {
        public void Execute(Player player, Entity weapon) => SelfEvent.Execute(this, player, weapon);

        public IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteRailgunChargingShotEvent>();
    }
}
