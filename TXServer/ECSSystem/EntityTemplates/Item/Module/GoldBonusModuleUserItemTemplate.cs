using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Item.Module
{
	[SerialVersionUID(1531929899999L)]
	public class GoldBonusModuleUserItemTemplate : IEntityTemplate
    {
		public static Entity CreateEntity(Entity garageModule, BattleTankPlayer battlePlayer)
        {
			Entity slot = battlePlayer.Player.CurrentPreset.Modules.SingleOrDefault(x => x.Value == garageModule).Key;
			Component slotUserItemInfoComponent = slot != null
				? slot.GetComponent<SlotUserItemInfoComponent>()
				: new SlotUserItemInfoComponent(Types.Slot.SLOT7, ModuleBehaviourType.ACTIVE);

			Entity entity = new(
				new TemplateAccessor(new GoldBonusModuleUserItemTemplate(), garageModule.TemplateAccessor.ConfigPath),
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
