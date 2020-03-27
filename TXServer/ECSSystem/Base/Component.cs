namespace TXServer.ECSSystem.Base
{
    public abstract class Component
    {
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
