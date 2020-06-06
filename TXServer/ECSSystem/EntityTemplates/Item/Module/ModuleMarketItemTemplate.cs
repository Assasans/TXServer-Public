using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1484905625943)]
    public class ModuleMarketItemTemplate : IMarketItemTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, TankPartModuleType part, int tier, ModuleBehaviourType behaviour, bool immutable = false)
        {
            Entity entity = new Entity(id, new TemplateAccessor(new ModuleMarketItemTemplate(), string.Format("garage/module/{0}module/", immutable ? "prebuild" : "") + configPathEntry),
                new ParentGroupComponent(id),
                new MarketItemGroupComponent(id),
                new ModuleTankPartComponent(part),
                new ModuleTierComponent(tier),
                new ModuleBehaviourTypeComponent(behaviour),
                new ModuleCardsCompositionComponent());
            if (immutable)
                entity.Components.Add(new ImmutableModuleItemComponent());

            return entity;
        }

        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            IEntityTemplate template = null;
            ModuleTankPartComponent part = marketItem.GetComponent<ModuleTankPartComponent>();

            if (part.TankPart == TankPartModuleType.COMMON)
            {
                template = new GoldBonusModuleUserItemTemplate();
            }
            else
            {
                switch (marketItem.GetComponent<ModuleBehaviourTypeComponent>().Type)
                {
                    case ModuleBehaviourType.ACTIVE:
                        template = new ModuleUserItemTemplate();
                        break;
                    case ModuleBehaviourType.PASSIVE:
                        template = marketItem.TemplateAccessor.ConfigPath.Contains("trigger") ? new PassiveModuleUserItemTemplate() : new TriggerModuleUserItemTemplate() as IEntityTemplate;
                        break;
                }
            }

            return ModuleUserItemTemplate.CreateEntity(template, marketItem, user);
        }
    }
}
