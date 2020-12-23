using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Battle;

namespace TXServer.Core.Battles
{
    public struct TranslatedEvent
    {
        public TranslatedEvent(IRemoteEvent @event, Entity tankPart)
        {
            Event = @event;
            TankPart = tankPart;
        }

        public IRemoteEvent Event { get; }
        public Entity TankPart { get; }
    }
}
