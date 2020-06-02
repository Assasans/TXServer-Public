using System;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    public interface IMarketItemTemplate : IEntityTemplate
    {
        Entity GetUserItem(Entity marketItem, Entity user);
    }
}
