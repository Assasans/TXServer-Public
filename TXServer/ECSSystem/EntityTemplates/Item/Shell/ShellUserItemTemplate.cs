using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(-1597888122960034653L)]
    public class ShellUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity item = new Entity(new TemplateAccessor(new ShellUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);
            item.Components.Add(new UserGroupComponent(user));

            return item;
        }
    }
}
