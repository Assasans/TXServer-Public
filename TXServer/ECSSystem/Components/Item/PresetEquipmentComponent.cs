using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1502886877871L)]
    public class PresetEquipmentComponent : Component
    {
        private PlayerPreset _preset;

        public PresetEquipmentComponent(PlayerPreset preset)
        {
            _preset = preset;
        }

        public long weaponId { get; set; }
        [ProtocolIgnore]
        public long WeaponId
        {
            set
            {
                weaponId = value;
                if (Preset.TryRemoveComponent<PresetEquipmentComponent>())
                    Preset.AddComponent(this);
            }
        }

        public long hullId { get; set; }

        [ProtocolIgnore]
        public long HullId
        {
            set
            {
                hullId = value;
                if (Preset.TryRemoveComponent<PresetEquipmentComponent>())
                    Preset.AddComponent(this);
            }
        }

        [ProtocolIgnore] public Entity Preset => _preset.Entity;
    }
}
