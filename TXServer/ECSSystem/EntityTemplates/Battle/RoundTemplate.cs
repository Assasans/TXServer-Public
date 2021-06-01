using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Time;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1429256309752)]
    public class RoundTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity battle)
        {
            return new Entity(new TemplateAccessor(new RoundTemplate(), null),
                new RoundComponent(),
                new BattleGroupComponent(battle),

                // WarmingUpTimerSystem
                new RoundStopTimeComponent(DateTimeOffset.UtcNow.AddSeconds(40)),
                new RoundActiveStateComponent()
            );
        }
    }
}
