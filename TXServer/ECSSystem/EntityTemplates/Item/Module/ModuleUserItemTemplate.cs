using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1484901449548L)]
    public class ModuleUserItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(IEntityTemplate template, Entity marketItem, Entity user)
        {
            Entity item = new Entity(new TemplateAccessor(template, marketItem.TemplateAccessor.ConfigPath), marketItem.Components);

            item.Components.UnionWith(new Component[]
            {
                new UserGroupComponent(user),
                new ModuleGroupComponent(item),
                new ModuleUpgradeLevelComponent(),
            });

            return item;
        }
    }
}
