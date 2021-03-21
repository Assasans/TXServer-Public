using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-8835994525014820133L)]
    public class KillEvent : ECSEvent
    {
        public KillEvent(Entity killerMarketItem, Entity target)
        {
            KillerMarketItem = killerMarketItem;
            Target = target;
        }

        public Entity KillerMarketItem { get; set; }

        public Entity Target { get; set; }
    }
}