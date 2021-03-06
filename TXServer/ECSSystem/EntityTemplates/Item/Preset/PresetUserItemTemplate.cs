using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1493972686116L)]
    public class PresetUserItemTemplate : IUserItemTemplate, IMountableItemTemplate
    {
        public void AddUserItemComponents(Player player, Entity item)
        {
            item.Components.Add(new PresetEquipmentComponent(player, item));
            item.Components.Add(new PresetNameComponent($"Preset {player.Data.Presets.Count + 1}"));
            player.Data.Presets.Add(item);
        }
    }
}
