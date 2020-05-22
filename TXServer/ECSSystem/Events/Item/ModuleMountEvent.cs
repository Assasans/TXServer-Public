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
		public void Execute(Entity module, Entity slot)
		{
			Player.Instance.CurrentPreset.Modules[slot] = module;

			ModuleGroupComponent component = new ModuleGroupComponent(module);

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentAddCommand(slot, component),
				new ComponentAddCommand(module, new MountedItemComponent()));
		}
	}
}
