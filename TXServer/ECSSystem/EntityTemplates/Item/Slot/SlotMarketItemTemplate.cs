using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636390988457169067L)]
    public class SlotMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            // All slots share same market item.
            throw new NotSupportedException();
        }
    }
}
