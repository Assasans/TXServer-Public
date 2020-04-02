using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1507791699587)]
    public class CountableItemsPackComponent : Component
    {
        public CountableItemsPackComponent(Dictionary<Entity, int> Pack)
        {
            this.Pack = Pack;
        }

        public Dictionary<Entity, int> Pack { get; set; }
    }
}
