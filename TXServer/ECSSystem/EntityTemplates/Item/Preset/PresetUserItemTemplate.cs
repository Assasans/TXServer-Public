using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1493972686116L)]
    public class PresetUserItemTemplate : IUserItemTemplate, IMountableItemTemplate
    {
        public void AddUserItemComponents(Entity item)
        {
            item.Components.Add(new PresetEquipmentComponent(item));
            item.Components.Add(new PresetNameComponent(Player.GenerateId().ToString()));
        }
    }
}
