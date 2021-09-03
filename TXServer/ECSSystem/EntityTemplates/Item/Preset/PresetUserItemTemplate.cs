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
            PlayerPreset preset = PlayerPreset.Create(player.Data, player.Data.Presets.Count, $"Preset {player.Data.Presets.Count + 1}");
            preset.Entity = item;

            item.Components.Add(new PresetEquipmentComponent(preset));
            item.Components.Add(new PresetNameComponent(preset));

            player.Data.Presets.Add(preset);
        }
    }
}
