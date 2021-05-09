using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1507791699587)]
    public class CountableItemsPackComponent : Component
    {
        public CountableItemsPackComponent(Dictionary<Entity, int> pack)
        {
            Pack = pack.ToDictionary(x => x.Key.EntityId, x => x.Value);
        }

        public Dictionary<long, int> Pack { get; set; }
    }
}
