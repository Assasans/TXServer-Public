using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1498740539984L)]
    public class LoadSortedFriendsIdsWithNicknamesEvent : ECSEvent
    {
        public void Execute(Entity clientSession)
        {
            CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new SortedFriendsIdsLoadedEvent(), clientSession));
        }
    }
}
