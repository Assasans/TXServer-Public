using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Item.Slot
{
    [SerialVersionUID(1485846188251L)]
    public class SlotUserItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity module, BattlePlayer battlePlayer)
        {
            Entity entity = new Entity(new TemplateAccessor(new SlotUserItemTemplate(), "/garage/module/slot"),
                module.GetComponent<SlotTankPartComponent>(),
                module.GetComponent<SlotUserItemInfoComponent>(),
                battlePlayer.MatchPlayer.Tank.GetComponent<UserGroupComponent>(),
                battlePlayer.MatchPlayer.Tank.GetComponent<TankGroupComponent>()
            );
            
            module.AddComponent(new ModuleGroupComponent(entity.EntityId));
            
            return entity;
        }
    }
}