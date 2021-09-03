using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(1485777098598L)]
	public class ModuleMountEvent : ECSEvent
	{
		public void Execute(Player player, Entity module, Entity slot)
        {
            if (slot.HasComponent<ModuleGroupComponent>() || module.HasComponent<MountedItemComponent>()) return;

            // Remove old module first
            PlayerPresetModule presetModule = player.CurrentPreset.Modules.SingleOrDefault(module => module.Slot == slot.GetComponent<SlotUserItemInfoComponent>().Slot);
            if(presetModule != null) player.CurrentPreset.Modules.Remove(presetModule);

            player.CurrentPreset.Modules.Add(PlayerPresetModule.Create(
                player.CurrentPreset,
                slot.GetComponent<SlotUserItemInfoComponent>().Slot,
                module.GetComponent<MarketItemGroupComponent>().Key
            ));

			slot.AddComponent(new ModuleGroupComponent(module));
			module.AddComponent(new MountedItemComponent());
		}
	}
}
