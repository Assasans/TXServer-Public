using System.Linq;

using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Module;

namespace TXServer.ECSSystem.EntityTemplates
{
	[SerialVersionUID(1486980355472L)]
	public class PassiveModuleUserItemTemplate : IEntityTemplate
    {
		public static Entity CreateEntity(Entity garageModule, BattleTankPlayer battlePlayer) {

			Entity slot = battlePlayer.Player.CurrentPreset.Modules.SingleOrDefault(x => x.Value == garageModule).Key;
			Component slotUserItemInfoComponent = slot.GetComponent<SlotUserItemInfoComponent>();

			Entity entity = new(
				new TemplateAccessor(
					new PassiveModuleUserItemTemplate(),
					garageModule.TemplateAccessor.ConfigPath
				),
				new SlotTankPartComponent(garageModule.GetComponent<ModuleTankPartComponent>().TankPart),
				slotUserItemInfoComponent,
                new ModuleUsesCounterComponent(),
                battlePlayer.MatchPlayer.TankEntity.GetComponent<UserGroupComponent>(),
				battlePlayer.MatchPlayer.TankEntity.GetComponent<TankGroupComponent>()
			);
			return entity;
		}
	}
}
