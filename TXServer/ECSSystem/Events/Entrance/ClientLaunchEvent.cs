using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.Core.ECSSystem.Events
{
    [SerialVersionUID(1478774431678)]
    public class ClientLaunchEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            // WebId message
            CommandManager.SendCommands(player,
                new ComponentAddCommand(entity, new WebIdComponent()),
                new ComponentChangeCommand(entity, new WebIdComponent())
            );
        }

        public string WebId { get; set; }
    }
}
