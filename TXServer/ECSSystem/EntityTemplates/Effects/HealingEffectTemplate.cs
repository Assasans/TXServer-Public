using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Effects
{
    [SerialVersionUID(1486988156885L)]
    public class HealingEffectTemplate : EffectBaseTemplate, IEntityTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = new(new TemplateAccessor(new HealingEffectTemplate(), "battle/effect/healing"),
                new EffectComponent(),
                new DurationConfigComponent(3000),
                new DurationComponent { StartedTime = DateTime.UtcNow },
                tank.GetComponent<TankGroupComponent>());
            return entity;
        }
    }
}
