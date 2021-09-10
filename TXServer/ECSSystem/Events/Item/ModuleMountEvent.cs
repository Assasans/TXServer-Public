using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1485777098598L)]
	public class ModuleMountEvent : ECSEvent
	{
		public void Execute(Player player, Entity module, Entity slot)
        {
            player.CurrentPreset.Modules[slot] = module;

            slot.TryAddComponent(new ModuleGroupComponent(module));
			module.TryAddComponent(new MountedItemComponent());
		}
	}
}
