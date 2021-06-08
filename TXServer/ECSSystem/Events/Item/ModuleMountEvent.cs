using TXServer.Core;
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
            if (slot.HasComponent<ModuleGroupComponent>() || module.HasComponent<MountedItemComponent>())
                return;

            player.CurrentPreset.Modules[slot] = module;

			slot.AddComponent(new ModuleGroupComponent(module));
			module.AddComponent(new MountedItemComponent());
		}
	}
}
