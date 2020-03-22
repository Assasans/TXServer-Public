namespace TXServer.Core.ECSSystem.Components
{
    public abstract class Component
    {
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
