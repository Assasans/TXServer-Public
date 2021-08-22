namespace TXServer.Core
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
