using System.Collections.Generic;
using System.Linq;
using Serilog;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1434530333851L)]
	public class MountItemEvent : ECSEvent
	{
        private static readonly ILogger Logger = Log.Logger.ForType<MountItemEvent>();

		public void Execute(Player player, Entity item)
		{
			Entity prevItem;

			switch (item.TemplateAccessor.Template)
			{
                case IWeaponUserItemTemplate _:
                {
                    var savedSkin = player.Data.Weapons.GetById(player.CurrentPreset.Weapon.EntityId).SkinId;
                    var savedShellSkin = player.Data.Weapons.GetById(player.CurrentPreset.Weapon.EntityId).ShellSkinId;

                    prevItem = player.CurrentPreset.GetPlayerWeapon(player);
                    player.CurrentPreset.Weapon = player.FindEntityById(item.GetComponent<ParentGroupComponent>().Key);
                    player.CurrentPreset.WeaponSkin = player.FindEntityById(savedSkin);
                    player.CurrentPreset.WeaponShellSkin = player.FindEntityById(savedShellSkin);
                    player.CurrentPreset.Entity.GetComponent<PresetEquipmentComponent>().WeaponId = player.CurrentPreset.Weapon.EntityId;
                    break;
                }
                case TankUserItemTemplate _:
                {
                    var savedSkin = player.Data.Hulls.GetById(player.CurrentPreset.Hull.EntityId).SkinId;

                    prevItem = player.CurrentPreset.GetPlayerHull(player);
                    player.CurrentPreset.Hull = player.FindEntityById(item.GetComponent<ParentGroupComponent>().Key);
                    player.CurrentPreset.HullSkin = player.FindEntityById(savedSkin);
                    player.CurrentPreset.Entity.GetComponent<PresetEquipmentComponent>().HullId = player.CurrentPreset.Hull.EntityId;
                    break;
                }
                case AvatarUserItemTemplate _:
                    prevItem = player.GetUserItemByMarket(player.GetEntityById(player.Data.Avatar));
                    player.Data.Avatar = player.GetMarketItemByUser(item).EntityId;
                    break;
				case TankPaintUserItemTemplate _:
					prevItem = player.CurrentPreset.GetPlayerHullPaint(player);
					player.CurrentPreset.HullPaint = player.GetMarketItemByUser(item);
					break;
				case WeaponPaintUserItemTemplate _:
					prevItem = player.CurrentPreset.GetPlayerWeaponPaint(player);
					player.CurrentPreset.WeaponPaint = player.GetMarketItemByUser(item);
					break;
                case WeaponSkinUserItemTemplate _:
                {
                    prevItem = player.CurrentPreset.GetPlayerWeaponSkin(player);
                    player.CurrentPreset.WeaponSkin = player.GetMarketItemByUser(item);
                    player.Data.Weapons.GetById(player.CurrentPreset.Weapon.EntityId).SkinId = player.CurrentPreset.WeaponSkin.EntityId;
                    break;
                }
                case HullSkinUserItemTemplate _:
                {
                    prevItem = player.CurrentPreset.GetPlayerHullSkin(player);
                    player.CurrentPreset.HullSkin = player.GetMarketItemByUser(item);
                    player.Data.Hulls.GetById(player.CurrentPreset.Hull.EntityId).SkinId = player.CurrentPreset.HullSkin.EntityId;
                    break;
                }
                case ShellUserItemTemplate _:
                {
                    prevItem = player.CurrentPreset.GetPlayerWeaponShellSkin(player);
                    player.CurrentPreset.WeaponShellSkin = player.GetMarketItemByUser(item);
                    player.Data.Weapons.GetById(player.CurrentPreset.Weapon.EntityId).ShellSkinId = player.CurrentPreset.WeaponShellSkin.EntityId;
                    break;
                }
                case GraffitiUserItemTemplate _:
					prevItem = player.CurrentPreset.Graffiti;
					player.CurrentPreset.Graffiti = player.GetMarketItemByUser(item);
					break;
				case PresetUserItemTemplate _:
					// Unmount previous preset items
					PlayerPreset prevPreset = player.CurrentPreset;

					foreach (Entity presetItem in new[]
					{
                        prevPreset.GetPlayerHull(player),
                        prevPreset.GetPlayerHullPaint(player),
                        prevPreset.GetPlayerHullSkin(player),
                        prevPreset.GetPlayerWeapon(player),
                        prevPreset.GetPlayerWeaponPaint(player),
                        prevPreset.GetPlayerWeaponSkin(player),
                        prevPreset.GetPlayerWeaponShellSkin(player),
                        prevPreset.GetPlayerGraffiti(player)
					}.Concat(prevPreset.GetPlayerModules(player).Values))
					{
                        if (presetItem == null) continue;
                        if (presetItem.HasComponent<MountedItemComponent>())
                            presetItem.RemoveComponent<MountedItemComponent>();
					}

                    foreach (Entity slot in prevPreset.GetPlayerModules(player).Keys)
                    {
                        if (slot.HasComponent<MountedItemComponent>())
                            slot.RemoveComponent<MountedItemComponent>();
                    }

                    // Mount new preset items
                    PlayerPreset newPreset = player.Data.Presets.Single(preset => preset.Entity.EntityId == item.EntityId);

					foreach (Entity presetItem in new[]
					{
                        newPreset.GetPlayerHull(player),
                        newPreset.GetPlayerHullPaint(player),
                        newPreset.GetPlayerHullSkin(player),
                        newPreset.GetPlayerWeapon(player),
                        newPreset.GetPlayerWeaponPaint(player),
                        newPreset.GetPlayerWeaponSkin(player),
                        newPreset.GetPlayerWeaponShellSkin(player),
                        newPreset.GetPlayerGraffiti(player)
					}.Concat(newPreset.GetPlayerModules(player).Values))
					{
                        if (presetItem == null) continue;
                        if (!presetItem.HasComponent<MountedItemComponent>())
						    presetItem.AddComponent(new MountedItemComponent());
					}

					foreach (KeyValuePair<Entity, Entity> slotModulePair in newPreset.GetPlayerModules(player))
					{
						if (slotModulePair.Value != null)
							slotModulePair.Key.AddComponent(new ModuleGroupComponent(slotModulePair.Value));
					}

                    Logger.WithPlayer(player).Debug(
                        "Changed preset {OldPreset} to {NewPreset}",
                        prevPreset.Name,
                        newPreset.Name
                    );

					prevItem = prevPreset.Entity;
                    player.Data.CurrentPresetIndex = player.Data.Presets.IndexOf(newPreset);
                    break;
				default:
                    return;
			}

			if (prevItem.HasComponent<MountedItemComponent>())
                prevItem.RemoveComponent<MountedItemComponent>();

            if (!item.HasComponent<MountedItemComponent>())
			    item.AddComponent(new MountedItemComponent());

			if (player.User.TryRemoveComponent<UserEquipmentComponent>())
				player.User.AddComponent(new UserEquipmentComponent(player.CurrentPreset.Weapon.EntityId, player.CurrentPreset.Hull.EntityId));
		}
	}
}
