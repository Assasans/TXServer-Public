using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core
{
    public interface IHullEquipment
    {
        public long HullId { get; set; }
    }

    public interface IWeaponEquipment
    {
        public long WeaponId { get; set; }
    }

    public class PlayerPresetModule : PlayerData.IEntity
    {
        public long PlayerId { get; set; }
        public int PresetIndex { get; set; }

        [ForeignKey("PlayerId, PresetIndex")]
        public virtual PlayerPreset Preset { get; set; }

        public Slot Slot { get; set; }
        public long EntityId { get; set; }

        public static PlayerPresetModule Create(PlayerPreset preset, Slot slot, long entityId)
        {
            return new PlayerPresetModule
            {
                Preset = preset,
                PlayerId = preset.PlayerId,
                PresetIndex = preset.Index,

                Slot = slot,
                EntityId = entityId
            };
        }
    }

    public class PlayerPreset
    {
        public static PlayerPreset Create(PlayerData player, int index, string? name = null)
        {
            PlayerPreset preset = new PlayerPreset()
            {
                Player = player,
                PlayerId = player.UniqueId,
                Index = index,
                Name = name
            };
            preset.InitDefault();

            return preset;
        }

        public void InitDefault()
        {
            Hull = Hulls.GlobalItems.Hunter;
            HullPaint = Paints.GlobalItems.Green;
            HullSkin = Hulls.DefaultSkins[Hull];

            Weapon = Weapons.GlobalItems.Smoky;
            WeaponPaint = Covers.GlobalItems.None;
            WeaponSkin = Weapons.DefaultSkins[Weapon];
            WeaponShellSkin = Shells.DefaultShells[Weapon];

            Graffiti = TXServer.ECSSystem.GlobalEntities.Graffiti.GlobalItems.Logo;

            // Modules = new List<PlayerPresetModule>
            // {
            //     PlayerPresetModule.Create(this, Slot.SLOT1, null),
            //     PlayerPresetModule.Create(this, Slot.SLOT2, null),
            //     PlayerPresetModule.Create(this, Slot.SLOT3, null),
            //     PlayerPresetModule.Create(this, Slot.SLOT4, null),
            //     PlayerPresetModule.Create(this, Slot.SLOT5, null),
            //     PlayerPresetModule.Create(this, Slot.SLOT6, null)
            // };
        }

        public Entity GetPlayerHull(Player player) => player.GetUserItemByMarket(Hull);
        public Entity GetPlayerHullPaint(Player player) => player.GetUserItemByMarket(HullPaint);
        public Entity GetPlayerHullSkin(Player player) => player.GetUserItemByMarket(HullSkin);

        public Entity GetPlayerWeapon(Player player) => player.GetUserItemByMarket(Weapon);
        public Entity GetPlayerWeaponPaint(Player player) => player.GetUserItemByMarket(WeaponPaint);
        public Entity GetPlayerWeaponSkin(Player player) => player.GetUserItemByMarket(WeaponSkin);
        public Entity GetPlayerWeaponShellSkin(Player player) => player.GetUserItemByMarket(WeaponShellSkin);

        public Entity GetPlayerGraffiti(Player player) => player.GetUserItemByMarket(Graffiti);

        public Dictionary<Entity, Entity> GetPlayerModules(Player player)
        {
            Entity[] allSlots = player.UserItems[typeof(ModuleSlots)].GetAllItems();
            Entity[] allModules = player.UserItems[typeof(Modules)].GetAllItems();

            Dictionary<Entity, Entity> dictionary = new Dictionary<Entity, Entity>();

            foreach (PlayerPresetModule module in Modules)
            {
                Entity slotEntity = allSlots.Single(entity => entity.GetComponent<SlotUserItemInfoComponent>().Slot == module.Slot);
                Entity moduleEntity = allModules.Single(entity => entity.GetComponent<MarketItemGroupComponent>().Key == module.EntityId);

                dictionary.Add(slotEntity, moduleEntity);
            }

            foreach (Entity slotEntity in allSlots)
            {
                if(dictionary.Any(pair => pair.Key.EntityId == slotEntity.EntityId)) continue;
                dictionary.Add(slotEntity, null);
            }

            return dictionary;
        }

        public long PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual PlayerData Player { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        [NotMapped] public Entity Entity { get; set; }

        public Entity Hull { get; set; }
        public Entity HullPaint { get; set; }
        public Entity HullSkin { get; set; }

        public Entity Weapon { get; set; }
        public Entity WeaponPaint { get; set; }
        public Entity WeaponSkin { get; set; }
        public Entity WeaponShellSkin { get; set; }

        public Entity Graffiti { get; set; }

        public List<PlayerPresetModule> Modules { get; set; } = new List<PlayerPresetModule>();
    }
}
