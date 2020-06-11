using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(1502886877871L)]
	public class PresetEquipmentComponent : Component
	{
        public PresetEquipmentComponent(Entity preset)
        {
            Preset = preset;

            Weapons weaponList = Player.Instance.UserItems[typeof(Weapons)] as Weapons;
            Hulls hullList = Player.Instance.UserItems[typeof(Hulls)] as Hulls;
            WeaponSkins weaponSkinList = Player.Instance.UserItems[typeof(WeaponSkins)] as WeaponSkins;
            HullSkins hullSkinList = Player.Instance.UserItems[typeof(HullSkins)] as HullSkins;
            Shells shellList = Player.Instance.UserItems[typeof(Shells)] as Shells;
            ModuleSlots moduleSlotList = Player.Instance.UserItems[typeof(ModuleSlots)] as ModuleSlots;

            WeaponSkins = new Dictionary<Entity, Entity>
            {
                { weaponList.Flamethrower, weaponSkinList.FlamethrowerM0 },
                { weaponList.Freeze, weaponSkinList.FreezeM0 },
                { weaponList.Hammer, weaponSkinList.HammerM0 },
                { weaponList.Isis, weaponSkinList.IsisM0 },
                { weaponList.Railgun, weaponSkinList.RailgunM0 },
                { weaponList.Ricochet, weaponSkinList.RicochetM0 },
                { weaponList.Shaft, weaponSkinList.ShaftM0 },
                { weaponList.Smoky, weaponSkinList.SmokyM0 },
                { weaponList.Thunder, weaponSkinList.ThunderM0 },
                { weaponList.Twins, weaponSkinList.TwinsM0 },
                { weaponList.Vulcan, weaponSkinList.VulcanM0 }
            };

            HullSkins = new Dictionary<Entity, Entity>
            {
                { hullList.Dictator, hullSkinList.DictatorM0 },
                { hullList.Hornet, hullSkinList.HornetM0 },
                { hullList.Hunter, hullSkinList.HunterM0 },
                { hullList.Mammoth, hullSkinList.MammothM0 },
                { hullList.Titan, hullSkinList.TitanM0 },
                { hullList.Viking, hullSkinList.VikingM0 },
                { hullList.Wasp, hullSkinList.WaspM0 }
            };

            WeaponShells = new Dictionary<Entity, Entity>
            {
                { weaponList.Flamethrower, shellList.FlamethrowerOrange },
                { weaponList.Freeze, shellList.FreezeSkyblue },
                { weaponList.Hammer, shellList.HammerStandard },
                { weaponList.Isis, shellList.IsisStandard },
                { weaponList.Railgun, shellList.RailgunPaleblue },
                { weaponList.Ricochet, shellList.RicochetAurulent },
                { weaponList.Shaft, shellList.ShaftStandard },
                { weaponList.Smoky, shellList.SmokyStandard },
                { weaponList.Thunder, shellList.ThunderStandard },
                { weaponList.Twins, shellList.TwinsBlue },
                { weaponList.Vulcan, shellList.VulcanStandard }
            };

            Modules = new Dictionary<Entity, Entity>
            {
                { moduleSlotList.Slot1, null },
                { moduleSlotList.Slot2, null },
                { moduleSlotList.Slot3, null },
                { moduleSlotList.Slot4, null },
                { moduleSlotList.Slot5, null },
                { moduleSlotList.Slot6, null }
            };
        }

		public Entity Weapon
        {
            get
            {
                WeaponItem.Components.TryGetValue(new ParentGroupComponent(0), out Component component);
                return Entity.FindById((component as ParentGroupComponent).Key);
            }
        }

		public Entity Hull
        {
            get
            {
                HullItem.Components.TryGetValue(new ParentGroupComponent(0), out Component component);
                return Entity.FindById((component as ParentGroupComponent).Key);
            }
        }

        [ProtocolIgnore] public Entity Preset { get; set; }

        [ProtocolIgnore] public Entity WeaponItem { get; set; } = (Player.Instance.UserItems[typeof(Weapons)] as Weapons).Smoky;
        [ProtocolIgnore] public Entity HullItem { get; set; } = (Player.Instance.UserItems[typeof(Hulls)] as Hulls).Hunter;

        [ProtocolIgnore] public Entity WeaponPaint { get; set; } = (Player.Instance.UserItems[typeof(Covers)] as Covers).None;
        [ProtocolIgnore] public Entity TankPaint { get; set; } = (Player.Instance.UserItems[typeof(Paints)] as Paints).Green;

        [ProtocolIgnore] public Dictionary<Entity, Entity> WeaponSkins { get; set; }
        [ProtocolIgnore] public Dictionary<Entity, Entity> HullSkins { get; set; }

        [ProtocolIgnore] public Dictionary<Entity, Entity> WeaponShells { get; set; }
        [ProtocolIgnore] public Entity Graffiti { get; set; } = (Player.Instance.UserItems[typeof(Graffiti)] as Graffiti).Logo;

        [ProtocolIgnore] public Dictionary<Entity, Entity> Modules { get; set; }
    }
}
