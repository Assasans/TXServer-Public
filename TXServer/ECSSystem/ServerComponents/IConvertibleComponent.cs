using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public interface IConvertibleComponent<T> where T : Component
    {
        void Convert(T component);
    }
}
