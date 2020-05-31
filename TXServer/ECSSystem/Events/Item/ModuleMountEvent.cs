using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1485777098598L)]
	public class ModuleMountEvent : ECSEvent
	{
		public void Execute(Player player, Entity module, Entity slot)
		{
			player.CurrentPreset.Modules[slot] = module;

			ModuleGroupComponent component = new ModuleGroupComponent(module);

			CommandManager.SendCommands(player,
				new ComponentAddCommand(slot, component),
				new ComponentAddCommand(module, new MountedItemComponent()));
		}
	}
}
