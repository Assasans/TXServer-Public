using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect.Unit
{
    [SerialVersionUID(1485231135123L)]
    public sealed class UnitGroupComponent : GroupComponent
    {
        public UnitGroupComponent(Entity Key) : base(Key)
        {
        }

        public UnitGroupComponent(long Key) : base(Key)
        {
        }
    }
}
