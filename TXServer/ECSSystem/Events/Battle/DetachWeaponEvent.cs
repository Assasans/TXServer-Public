using System.Numerics;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1447917521601L)]
    public class DetachWeaponEvent : ECSEvent, ISelfEvent, IRemoteEvent
    {
        public void Execute(Player player, Entity tank) => SelfEvent.Execute(this, player, tank);

        public Vector3 AngularVelocity { get; set; }

        public Vector3 Velocity { get; set; }

        public IRemoteEvent ToRemoteEvent() => this;
    }
}