using System.Linq;

using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Module;

namespace TXServer.ECSSystem.EntityTemplates {
	[SerialVersionUID(636304361927229412L)]
	public class TriggerModuleUserItemTemplate : IEntityTemplate {
		public static Entity CreateEntity(Entity garageModule, BattleTankPlayer battlePlayer) {
			Entity slot = battlePlayer.Player.CurrentPreset.Modules.SingleOrDefault(x => x.Value == garageModule).Key;
			Component slotUserItemInfoComponent = slot.GetComponent<SlotUserItemInfoComponent>();

			Entity entity = new(
				new TemplateAccessor(
					new TriggerModuleUserItemTemplate(),
					garageModule.TemplateAccessor.ConfigPath
				),
				new SlotTankPartComponent(garageModule.GetComponent<ModuleTankPartComponent>().TankPart),
				slotUserItemInfoComponent,
                new InventoryEnabledStateComponent(),
				new ModuleUsesCounterComponent(),
				//new UserItemCounterComponent(100),
				battlePlayer.MatchPlayer.Tank.GetComponent<UserGroupComponent>(),
				battlePlayer.MatchPlayer.Tank.GetComponent<TankGroupComponent>()
			);
			return entity;
		}
	}
}
