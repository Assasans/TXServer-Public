using System.Linq;

using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Item.Module {
	[SerialVersionUID(1484901449548L)]
	public class ModuleUserItemTemplate : IEntityTemplate {
		public static Entity CreateEntity(Entity garageModule, BattleTankPlayer battlePlayer)
        {
			Entity slot = battlePlayer.Player.CurrentPreset.GetPlayerModules(battlePlayer.Player).SingleOrDefault(x => x.Value == garageModule).Key;
            if (slot is null) return null;
			Component slotUserItemInfoComponent = slot.GetComponent<SlotUserItemInfoComponent>();

			Entity entity = new(new TemplateAccessor(new ModuleUserItemTemplate(), garageModule.TemplateAccessor.ConfigPath),
				new SlotTankPartComponent(garageModule.GetComponent<ModuleTankPartComponent>().TankPart),
				// new ModuleTierComponent(1),
				slotUserItemInfoComponent,
				/*
				new SlotUserItemInfoComponent(slot.GetComponent<SlotUserItemInfoComponent>().Slot, garageModule.GetComponent<ModuleBehaviourTypeComponent>().Type) {
				    UpgradeLevel = 1
				},
				*/
                /*
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleUpgradeLevelComponent() {
                    Level = 666
                },
                */
				new ModuleUsesCounterComponent(),
				//new UserItemCounterComponent(100),
				battlePlayer.MatchPlayer.Tank.GetComponent<UserGroupComponent>(),
				battlePlayer.MatchPlayer.Tank.GetComponent<TankGroupComponent>()
			);

            if (garageModule.GetComponent<ModuleBehaviourTypeComponent>().Type is not ModuleBehaviourType.PASSIVE)
                entity.AddComponent(new InventoryEnabledStateComponent());

			return entity;
		}
	}
}
