using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1543968970810L)]
    public class SimpleChestUserItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user) => ContainerUserItemTemplate.CreateEntity(new SimpleChestUserItemTemplate(), marketItem, user);
    }
}
