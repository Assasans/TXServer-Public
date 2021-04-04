using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates.Effects
{
    [SerialVersionUID(1486018775542L)]
    public class ArmorEffectTemplate : EffectBaseTemplate, IEntityTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = new(new TemplateAccessor(new ArmorEffectTemplate(), "battle/effect/armor"),
                new EffectComponent(),
                new DurationConfigComponent(30000),
                new DurationComponent(new TXDate(DateTimeOffset.Now)),
                tank.GetComponent<TankGroupComponent>());
            return entity;
        }
    }
}