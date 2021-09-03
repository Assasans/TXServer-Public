using System.Linq;
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
            if (!slot.HasComponent<ModuleGroupComponent>() || !module.HasComponent<MountedItemComponent>())
                return;

            PlayerPresetModule presetModule = player.CurrentPreset.Modules.Single(module => module.Slot == slot.GetComponent<SlotUserItemInfoComponent>().Slot);
            player.CurrentPreset.Modules.Remove(presetModule);

			slot.RemoveComponent<ModuleGroupComponent>();
			module.RemoveComponent<MountedItemComponent>();
		}
	}
}
