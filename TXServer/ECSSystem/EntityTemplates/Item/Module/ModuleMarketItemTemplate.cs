using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1484905625943)]
    public class ModuleMarketItemTemplate : IMarketItemTemplate
    {
        public Entity GetUserItem(Entity marketItem, Entity user)
        {
            IEntityTemplate template = null;
            ModuleTankPartComponent part = marketItem.GetComponent<ModuleTankPartComponent>();

            if (part.TankPart == Types.TankPartModuleType.COMMON)
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
