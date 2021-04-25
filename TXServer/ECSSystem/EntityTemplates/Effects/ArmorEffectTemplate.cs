using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Effects
{
    [SerialVersionUID(1486018775542L)]
    public class ArmorEffectTemplate : EffectBaseTemplate, IEntityTemplate
    {
        public static Entity CreateEntity(long duration, Entity tank)
        {
            Entity entity = new(new TemplateAccessor(new ArmorEffectTemplate(), "battle/effect/armor"),
                new EffectComponent(),
                new DurationConfigComponent(duration),
                new DurationComponent { StartedTime = DateTime.Now },
                tank.GetComponent<TankGroupComponent>());
            return entity;
        }
    }
}