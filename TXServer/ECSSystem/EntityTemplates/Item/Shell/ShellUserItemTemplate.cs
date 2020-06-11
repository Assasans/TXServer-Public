using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(-1597888122960034653L)]
    public class ShellUserItemTemplate : UserItemTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity userItem = new Entity(new TemplateAccessor(new ShellUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);
            userItem.Components.Add(new UserGroupComponent(user));

            AddToUserItems(typeof(Shells), userItem);
            return userItem;
        }
    }
}
