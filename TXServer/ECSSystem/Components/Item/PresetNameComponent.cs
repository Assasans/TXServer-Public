using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1493974995307L)]
    public class PresetNameComponent : Component
    {
        public PresetNameComponent(PlayerPreset preset)
        {
            _preset = preset;
        }

        public void OnChanged(Player player, Entity preset)
        {
            _preset ??= player.Data.Presets.Single(playerPreset => playerPreset.Entity.EntityId == preset.EntityId);

            // If preset name has been changed before OnAttached was invoked
            if (_name != null) _preset.Name = _name;
        }

        private PlayerPreset _preset;

        private string _name;
        public string Name
        {
            get => _preset != null ? _preset.Name : _name;
            set
            {
                if (_preset != null) _preset.Name = value;
                else _name = value;
            }
        }
    }
}
