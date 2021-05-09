using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public abstract class RangedComponent : Component
    {
        public float InitialValue { get; set; }
        public float FinalValue { get; set; }
    }
}
