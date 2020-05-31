using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1451368548585L)]
    public class RequestLoadUserProfileEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            CommandManager.SendCommands(player, new SendEventCommand(new UserProfileLoadedEvent()));
        }

        public long UserId { get; set; }
    }
}
