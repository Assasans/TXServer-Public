using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Effects
{
    [SerialVersionUID(1486970297039L)]
    public class TurboSpeedEffectTemplate : EffectBaseTemplate, IEntityTemplate
    {
        public static Entity CreateEntity(long duration, Entity tank)
        {
            Entity entity = new(new TemplateAccessor(new TurboSpeedEffectTemplate(), "battle/effect/turbospeed"),
                new EffectComponent(),
                new DurationConfigComponent(duration),
                new DurationComponent { StartedTime = DateTime.UtcNow },
                tank.GetComponent<TankGroupComponent>());
            return entity;
        }
    }
}
