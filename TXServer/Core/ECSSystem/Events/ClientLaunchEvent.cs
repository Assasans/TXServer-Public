using System.IO;
using TXServer.Core.Commands;
using TXServer.Core.ECSSystem.Components;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1478774431678)]
    public class ClientLaunchEvent : ECSEvent
    {
        public override void Execute(Entity entity)
        {
            // WebId message
            CommandManager.SendCommands(Player.Instance.Socket,
                new ComponentAddCommand(entity, new WebIdComponent()),
                new ComponentChangeCommand(entity, new WebIdComponent())
            );
        }

        public string WebId { get; set; }
    }
}
