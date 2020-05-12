using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458555309592L)]
    public class UnLoadUsersEvent : ECSEvent
    {
        public void Execute(Entity entity)
        {
            List<Command> commands = new List<Command>();

            foreach (Entity toUnload in Users)
            {
                commands.Add(new EntityUnshareCommand(toUnload));
            }

            CommandManager.SendCommands(Player.Instance.Socket, commands.ToArray());
        }

        public HashSet<Entity> Users { get; set; }
    }
}
