using System.Linq;

using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Module;

namespace TXServer.ECSSystem.EntityTemplates {
	[SerialVersionUID(1531929899999L)]
	public class GoldBonusModuleUserItemTemplate : IEntityTemplate {
		public static Entity CreateEntity(Entity garageModule, BattleTankPlayer battlePlayer) {
			Entity slot = battlePlayer.Player.CurrentPreset.Modules.SingleOrDefault(x => x.Value == garageModule).Key;
			Component slotUserItemInfoComponent = slot != null
				? slot.GetComponent<SlotUserItemInfoComponent>()
				: new SlotUserItemInfoComponent(Slot.SLOT7, ModuleBehaviourType.ACTIVE);

			Entity entity = new(
				new TemplateAccessor(
					new GoldBonusModuleUserItemTemplate(),
					garageModule.TemplateAccessor.ConfigPath
				),
				new SlotTankPartComponent(garageModule.GetComponent<ModuleTankPartComponent>().TankPart),
				slotUserItemInfoComponent,
				new InventoryAmmunitionComponent(2),
				new InventoryEnabledStateComponent(),
				new ModuleUsesCounterComponent(),
				new UserItemCounterComponent(100),
				battlePlayer.MatchPlayer.Tank.GetComponent<UserGroupComponent>(),
				battlePlayer.MatchPlayer.Tank.GetComponent<TankGroupComponent>()
			);
			return entity;
		}
		
		// "/garage/module/prebuildmodule/common/active/1/gold"
	}
}
