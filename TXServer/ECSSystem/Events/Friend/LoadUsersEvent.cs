using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458555246853L)]
    public class LoadUsersEvent : ECSEvent
    {
        public void Execute(Entity entity)
        {
            List<Command> commands = new List<Command>();

            foreach (long id in UsersId)
            {
                Entity found;
                try
                {
                    found = ServerLauncher.Pool
                        .Where(player => player.User?.EntityId != Player.Instance.User.EntityId && player.User?.EntityId == id)
                        .Select(player => player.User)
                        .Single();
                }
                catch (InvalidOperationException)
                {
                    found = UserTemplate.CreateEntity("null");
                }

                commands.Add(new EntityShareCommand(found));
            }

            commands.Add(new SendEventCommand(new UsersLoadedEvent(RequestEntityId), entity));

            lock (Player.Instance.EntityList)
                CommandManager.SendCommands(Player.Instance.Socket, commands.ToArray());
        }

        public long RequestEntityId { get; set; }

        public HashSet<long> UsersId { get; set; }
    }
}
