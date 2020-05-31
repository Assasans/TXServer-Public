using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1485777830853L)]
	public class UnmountModuleFromSlotEvent : ECSEvent
	{
		public void Execute(Player player, Entity module, Entity slot)
		{
			player.CurrentPreset.Modules[slot] = null;

			CommandManager.SendCommands(player,
				new ComponentRemoveCommand(slot, typeof(ModuleGroupComponent)),
				new ComponentRemoveCommand(module, typeof(MountedItemComponent)));
		}
	}
}
