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
		public void Execute(Entity module, Entity slot)
		{
			Player.Instance.CurrentPreset.Modules[slot] = null;

			CommandManager.SendCommands(Player.Instance.Socket,
				new ComponentRemoveCommand(slot, typeof(ModuleGroupComponent)));
		}
	}
}
