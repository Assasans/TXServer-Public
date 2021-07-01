using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Lobby
{
    [SerialVersionUID(1496905821016L)]
    public class SetEquipmentEvent : ECSEvent
    {
        public void Execute(Player player, Entity lobby)
        {
        }

        public long WeaponId { get; set; }
        public long HullId { get; set; }
    }
}
