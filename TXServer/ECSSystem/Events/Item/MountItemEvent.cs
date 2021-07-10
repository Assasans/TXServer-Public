using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1434530333851L)]
	public class MountItemEvent : ECSEvent
	{
		public void Execute(Player player, Entity item)
		{
			Entity prevItem;

			switch (item.TemplateAccessor.Template)
			{
				case IWeaponUserItemTemplate _:
					prevItem = player.CurrentPreset.WeaponItem;
                    player.CurrentPreset.WeaponItem = item;
                    player.CurrentPreset.Weapon = player.FindEntityById(item.GetComponent<ParentGroupComponent>().Key);
                    player.CurrentPreset.WeaponId = player.CurrentPreset.Weapon.EntityId;
                    break;
				case TankUserItemTemplate _:
					prevItem = player.CurrentPreset.HullItem;
					player.CurrentPreset.HullItem = item;
                    player.CurrentPreset.Hull = player.FindEntityById(item.GetComponent<ParentGroupComponent>().Key);
                    player.CurrentPreset.HullId = player.CurrentPreset.Hull.EntityId;
					break;
				case AvatarUserItemTemplate _:
					prevItem = player.CurrentAvatar;
					player.CurrentAvatar = item;
                    player.Data.Avatar = item.EntityId;
                    break;
				case TankPaintUserItemTemplate _:
					prevItem = player.CurrentPreset.TankPaint;
					player.CurrentPreset.TankPaint = item;
					break;
				case WeaponPaintUserItemTemplate _:
					prevItem = player.CurrentPreset.WeaponPaint;
					player.CurrentPreset.WeaponPaint = item;
					break;
				case WeaponSkinUserItemTemplate _:
					prevItem = player.CurrentPreset.WeaponSkins[player.CurrentPreset.WeaponItem];
					player.CurrentPreset.WeaponSkins[player.CurrentPreset.WeaponItem] = item;
					break;
				case HullSkinUserItemTemplate _:
					prevItem = player.CurrentPreset.HullSkins[player.CurrentPreset.HullItem];
					player.CurrentPreset.HullSkins[player.CurrentPreset.HullItem] = item;
					break;
				case ShellUserItemTemplate _:
					prevItem = player.CurrentPreset.WeaponShells[player.CurrentPreset.WeaponItem];
					player.CurrentPreset.WeaponShells[player.CurrentPreset.WeaponItem] = item;
					break;
				case GraffitiUserItemTemplate _:
					prevItem = player.CurrentPreset.Graffiti;
					player.CurrentPreset.Graffiti = item;
					break;
				case PresetUserItemTemplate _:
					// Unmount previous preset items
					PresetEquipmentComponent prevPreset = player.CurrentPreset;

					foreach (Entity presetItem in new[]
					{
						prevPreset.HullItem,
						prevPreset.WeaponItem,
						prevPreset.TankPaint,
						prevPreset.WeaponPaint,
						prevPreset.Graffiti,
					}.Concat(prevPreset.HullSkins.Values)
					 .Concat(prevPreset.WeaponSkins.Values)
					 .Concat(prevPreset.WeaponShells.Values)
					 .Concat(prevPreset.Modules.Values))
					{
						presetItem?.RemoveComponent<MountedItemComponent>();
					}

					foreach (Entity slot in prevPreset.Modules.Keys)
						slot.TryRemoveComponent<ModuleGroupComponent>();

					// Mount new preset items
					PresetEquipmentComponent newPreset = item.GetComponent<PresetEquipmentComponent>();
					foreach (Entity presetItem in new[]
					{
						newPreset.HullItem,
						newPreset.WeaponItem,
						newPreset.TankPaint,
						newPreset.WeaponPaint,
						newPreset.Graffiti,
					}.Concat(newPreset.HullSkins.Values)
					 .Concat(newPreset.WeaponSkins.Values)
					 .Concat(newPreset.WeaponShells.Values)
					 .Concat(newPreset.Modules.Values))
					{
						presetItem?.AddComponent(new MountedItemComponent());
					}

					foreach (KeyValuePair<Entity, Entity> slotModulePair in newPreset.Modules)
					{
						if (slotModulePair.Value != null)
							slotModulePair.Key.AddComponent(new ModuleGroupComponent(slotModulePair.Value));
					}

					prevItem = prevPreset.Preset;
                    break;
				default:
                    return;
			}

			prevItem.RemoveComponent<MountedItemComponent>();
			item.AddComponent(new MountedItemComponent());

			if (player.User.TryRemoveComponent<UserEquipmentComponent>())
				player.User.AddComponent(new UserEquipmentComponent(player.CurrentPreset.Weapon.EntityId, player.CurrentPreset.Hull.EntityId));
		}
	}
}
