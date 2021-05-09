using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public interface IConvertibleComponent
    {
        Component Convert();
    }
}
