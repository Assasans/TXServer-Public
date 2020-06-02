using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1493972686116L)]
    public class PresetUserItemTemplate : IEntityTemplate, IMountableItemTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user)
        {
            Entity item = new Entity(new TemplateAccessor(new PresetUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);

            item.Components.UnionWith(new Component[]
            {
                new PresetEquipmentComponent(item),
                new PresetNameComponent(Player.GenerateId().ToString()),
                new UserGroupComponent(user)
            });

            return item;
        }
    }
}
