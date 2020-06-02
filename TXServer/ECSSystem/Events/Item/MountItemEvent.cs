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
		public void Execute(Entity item)
		{
			Entity prevItem;

			switch (item.TemplateAccessor.Template)
			{
				case WeaponUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.WeaponItem;
					Player.Instance.CurrentPreset.WeaponItem = item;
					break;
				case TankUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.HullItem;
					Player.Instance.CurrentPreset.HullItem = item;
					break;
				case AvatarUserItemTemplate _:
					Player.Instance.ReferencedEntities.TryGetValue("CurrentAvatar", out prevItem);
					Player.Instance.ReferencedEntities["CurrentAvatar"] = item;
					break;
				case TankPaintUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.TankPaint;
					Player.Instance.CurrentPreset.TankPaint = item;
					break;
				case WeaponPaintUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.WeaponPaint;
					Player.Instance.CurrentPreset.WeaponPaint = item;
					break;
				case WeaponSkinUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.WeaponSkins[Player.Instance.CurrentPreset.WeaponItem];
					Player.Instance.CurrentPreset.WeaponSkins[Player.Instance.CurrentPreset.WeaponItem] = item;
					break;
				case HullSkinUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.HullSkins[Player.Instance.CurrentPreset.HullItem];
					Player.Instance.CurrentPreset.HullSkins[Player.Instance.CurrentPreset.HullItem] = item;
					break;
				case ShellUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.WeaponShells[Player.Instance.CurrentPreset.WeaponItem];
					Player.Instance.CurrentPreset.WeaponShells[Player.Instance.CurrentPreset.WeaponItem] = item;
					break;
				case GraffitiUserItemTemplate _:
					prevItem = Player.Instance.CurrentPreset.Graffiti;
					Player.Instance.CurrentPreset.Graffiti = item;
					break;
				case PresetUserItemTemplate _:
					LinkedList<Command> commands = new LinkedList<Command>();

					// Unmount previous preset items
					PresetEquipmentComponent preset = Player.Instance.CurrentPreset;
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

					CommandManager.SendCommands(Player.Instance.Socket, commands);

					prevItem = preset.Preset;
					Player.Instance.CurrentPreset = newPreset;
					break;
				default:
					throw new NotImplementedException();
			}

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentRemoveCommand(prevItem, typeof(MountedItemComponent)),
				new ComponentAddCommand(item, new MountedItemComponent()));
		}
	}
}
