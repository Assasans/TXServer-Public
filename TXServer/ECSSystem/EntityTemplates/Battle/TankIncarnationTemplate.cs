using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(1478091203635)]
    public class TankIncarnationTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            return new Entity(new TemplateAccessor(new TankIncarnationTemplate(), null),
                new TankIncarnationComponent(),
                new TankGroupComponent(tank),
                new TankIncarnationKillStatisticsComponent());
        }
    }
}