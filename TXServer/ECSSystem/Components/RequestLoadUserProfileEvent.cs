using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1451368548585L)]
    public class RequestLoadUserProfileEvent : ECSEvent
    {
        public void Execute(Entity entity)
        {
            CommandManager.SendCommands(Player.Instance.Socket, new SendEventCommand(new UserProfileLoadedEvent()));
        }

        public long UserId { get; set; }
    }
}
