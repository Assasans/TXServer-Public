using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Unit
{
    [SerialVersionUID(1486455226183L)]
    public class UnitTargetComponent : Component
    {
        public UnitTargetComponent(Entity target, Entity targetIncarnation)
        {
            Target = target;
            TargetIncarnation = targetIncarnation;
        }

        public Entity Target { get; set; }
        public Entity TargetIncarnation { get; set; }
    }
}
