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
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = new(new TemplateAccessor(new TurboSpeedEffectTemplate(), "battle/effect/turbospeed"),
                new EffectComponent(),
                new DurationConfigComponent(30000),
                new DurationComponent { StartedTime = DateTime.Now },
                tank.GetComponent<TankGroupComponent>());
            return entity;
        }
    }
}