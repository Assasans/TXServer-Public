using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636319308428353334L)]
    public class ModuleCardUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new ModuleCardUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);

            userItem.Components.UnionWith(new Component[]
            {
                new UserGroupComponent(user),
                new UserItemCounterComponent(0)
            });

            return userItem;
        }
    }
}
