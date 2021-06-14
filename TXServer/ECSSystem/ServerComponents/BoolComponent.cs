using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public abstract class BoolComponent : Component
    {
        public bool Value { get; set; }
    }
}
