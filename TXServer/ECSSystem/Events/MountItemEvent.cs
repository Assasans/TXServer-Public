using System;
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
				case IWeaponUserItemTemplate _:
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
				default:
					throw new NotImplementedException();
			}

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentRemoveCommand(prevItem, typeof(MountedItemComponent)),
				new ComponentAddCommand(item, new MountedItemComponent()));
		}
	}
}
