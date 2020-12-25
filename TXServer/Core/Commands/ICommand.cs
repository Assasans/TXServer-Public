namespace TXServer.Core.Commands
{
    public interface ICommand
    {
        void OnSend(Player player);

        void OnReceive(Player player);
    }
}
