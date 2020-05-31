using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
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
					break;
				case TankUserItemTemplate _:
					prevItem = player.CurrentPreset.HullItem;
					player.CurrentPreset.HullItem = item;
					break;
				case AvatarUserItemTemplate _:
					player.ReferencedEntities.TryGetValue("CurrentAvatar", out prevItem);
					player.ReferencedEntities["CurrentAvatar"] = item;
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
					LinkedList<Command> commands = new LinkedList<Command>();

					// Unmount previous preset items
					PresetEquipmentComponent preset = player.CurrentPreset;
					List<Entity> entities = new List<Entity>
					{
						preset.HullItem,
						preset.WeaponItem,
						preset.TankPaint,
						preset.WeaponPaint,
						preset.Graffiti,
					};
					entities.AddRange(preset.HullSkins.Values);
					entities.AddRange(preset.WeaponSkins.Values);
					entities.AddRange(preset.WeaponShells.Values);
					entities.AddRange(preset.Modules.Values);

					foreach (Entity entity in entities)
					{
						if (entity != null)
							commands.AddLast(new ComponentRemoveCommand(entity, typeof(MountedItemComponent)));
					}

					foreach (KeyValuePair<Entity, Entity> pair in preset.Modules)
					{
						if (pair.Key.Components.Contains(new ModuleGroupComponent(0)))
						{
							commands.AddLast(new ComponentRemoveCommand(pair.Key, typeof(ModuleGroupComponent)));
						}
					}

					// Mount new preset items
					item.Components.TryGetValue(FormatterServices.GetUninitializedObject(typeof(PresetEquipmentComponent)) as PresetEquipmentComponent, out Component component);
					PresetEquipmentComponent newPreset = component as PresetEquipmentComponent;
					entities = new List<Entity>
					{
						newPreset.HullItem,
						newPreset.WeaponItem,
						newPreset.TankPaint,
						newPreset.WeaponPaint,
						newPreset.Graffiti,
					};
					entities.AddRange(newPreset.HullSkins.Values);
					entities.AddRange(newPreset.WeaponSkins.Values);
					entities.AddRange(newPreset.WeaponShells.Values);
					entities.AddRange(newPreset.Modules.Values);

					foreach (Entity entity in entities)
					{
						if (entity != null)
							commands.AddLast(new ComponentAddCommand(entity, new MountedItemComponent()));
					}

					foreach (KeyValuePair<Entity, Entity> pair in newPreset.Modules)
					{
						if (pair.Value != null)
						{
							commands.AddLast(new ComponentAddCommand(pair.Key, new ModuleGroupComponent(pair.Value)));
						}
					}

					CommandManager.SendCommands(player, commands);

					prevItem = preset.Preset;
					player.CurrentPreset = newPreset;
					break;
				default:
					throw new NotImplementedException();
			}

			CommandManager.SendCommands(player,
				new ComponentRemoveCommand(prevItem, typeof(MountedItemComponent)),
				new ComponentAddCommand(item, new MountedItemComponent()));
		}
	}
}
