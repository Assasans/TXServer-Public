using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(-6274985110858845212L)]
    public class StreamHitComponent : Component
    {
        [OptionalMapped]
        public HitTarget TankHit { get; set; }

        [OptionalMapped]
        public StaticHit StaticHit { get; set; }
    }
}
