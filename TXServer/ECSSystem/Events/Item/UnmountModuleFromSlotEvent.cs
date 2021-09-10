using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1485777830853L)]
	public class UnmountModuleFromSlotEvent : ECSEvent
	{
		public void Execute(Player player, Entity module, Entity slot)
		{
            player.CurrentPreset.Modules[slot] = null;

			slot.TryRemoveComponent<ModuleGroupComponent>();
			module.TryRemoveComponent<MountedItemComponent>();
		}
	}
}
