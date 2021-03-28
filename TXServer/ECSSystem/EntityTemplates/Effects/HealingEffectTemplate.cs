using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1486988156885L)]
    public class HealingEffectTemplate : EffectBaseTemplate, IEntityTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = new(new TemplateAccessor(new HealingEffectTemplate(), "battle/effect/healing"),
                new EffectComponent(),
                new DurationConfigComponent(3000),
                new DurationComponent(new TXDate(DateTimeOffset.Now)),
                tank.GetComponent<TankGroupComponent>());
            return entity;
        }
    }
}