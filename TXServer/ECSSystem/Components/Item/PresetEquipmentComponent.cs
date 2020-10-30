﻿using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components
{
	[SerialVersionUID(1502886877871L)]
	public class PresetEquipmentComponent : Component
    {
        private Player Player;
        
        public PresetEquipmentComponent(Player player, Entity preset)
        {
            Player = player;
            Preset = preset;

            WeaponItem = (Player.UserItems["Weapons"] as Weapons.Items).Smoky;
            HullItem = (Player.UserItems["Hulls"] as Hulls.Items).Hunter;

            WeaponPaint = (Player.UserItems["Covers"] as Covers.Items).None;
            TankPaint = (Player.UserItems["Paints"] as Paints.Items).Green;

            Graffiti = (Player.UserItems["Graffiti"] as Graffiti.Items).Logo;

            Weapons.Items weaponList = player.UserItems["Weapons"] as Weapons.Items;
            Hulls.Items hullList = player.UserItems["Hulls"] as Hulls.Items;
            WeaponSkins.Items weaponSkinList = player.UserItems["WeaponSkins"] as WeaponSkins.Items;
            HullSkins.Items hullSkinList = player.UserItems["HullSkins"] as HullSkins.Items;
            Shells.Items shellList = player.UserItems["Shells"] as Shells.Items;
            ModuleSlots.Items moduleSlotList = player.UserItems["ModuleSlots"] as ModuleSlots.Items;

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

        public Entity Weapon => Player.FindEntityById(WeaponItem.GetComponent<ParentGroupComponent>().Key);

        public Entity Hull => Player.FindEntityById(HullItem.GetComponent<ParentGroupComponent>().Key);

        [ProtocolIgnore] public Entity Preset { get; set; }

        [ProtocolIgnore] public Entity WeaponItem { get; set; }
        [ProtocolIgnore] public Entity HullItem { get; set; }

        [ProtocolIgnore] public Entity WeaponPaint { get; set; }
        [ProtocolIgnore] public Entity TankPaint { get; set; }

        [ProtocolIgnore] public Entity Graffiti { get; set; }

        // commented code below breaks mount item system
        /*
        [ProtocolIgnore] public Entity WeaponItem
        {
            get => (Player.UserItems["Weapons"] as Weapons.Items).Smoky;
            set { /* ignored rn / }
        }
        
        [ProtocolIgnore]
        public Entity HullItem
        {
            get => (Player.UserItems["Hulls"] as Hulls.Items).Hunter;
            set { /* ignored rn / }
        }

        [ProtocolIgnore]
        public Entity WeaponPaint
        {
            get => (Player.UserItems["Covers"] as Covers.Items).None;
            set { /* ignored rn / }
        }

        [ProtocolIgnore]
        public Entity TankPaint
        {
            get => (Player.UserItems["Paints"] as Paints.Items).Green;
            set { /* ignored rn / }
        }
        */

        [ProtocolIgnore] public Dictionary<Entity, Entity> WeaponSkins { get; set; }
        [ProtocolIgnore] public Dictionary<Entity, Entity> HullSkins { get; set; }

        [ProtocolIgnore] public Dictionary<Entity, Entity> WeaponShells { get; set; }

        /*
        [ProtocolIgnore]
        public Entity Graffiti
        {
            get => (Player.UserItems["Graffiti"] as Graffiti.Items).Logo;
            set { /* ignored rn / }
        }
        */

        [ProtocolIgnore] public Dictionary<Entity, Entity> Modules { get; set; }
    }
}
