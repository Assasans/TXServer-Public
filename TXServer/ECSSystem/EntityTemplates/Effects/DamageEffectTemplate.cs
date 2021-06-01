using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Effects
{
    [SerialVersionUID(1486018791920L)]
    public class DamageEffectTemplate : EffectBaseTemplate, IEntityTemplate
    {
        public static Entity CreateEntity(long duration, Entity tank)
        {
            Entity entity = new(new TemplateAccessor(new DamageEffectTemplate(), "battle/effect/damage"),
                new EffectComponent(),
                new DurationConfigComponent(duration),
                new DurationComponent { StartedTime = DateTime.UtcNow },
                tank.GetComponent<TankGroupComponent>());
            return entity;
        }
    }
}
