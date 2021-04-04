namespace TXServer.Core.Commands
{
    public interface ICommand
    {
        void OnReceive(Player player);
    }
}
