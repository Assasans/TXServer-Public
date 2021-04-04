using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(1)]
    public class SendEventCommand : ICommand
    {
        [ProtocolFixed] public ECSEvent Event { get; set; }
        [ProtocolFixed] public Entity[] Entities { get; set; }

        public SendEventCommand(ECSEvent @event, params Entity[] entities)
        {
            Event = @event;
            Entities = entities;
        }

        /// <summary>
        /// Finds event method with same number of Entity parameters and calls it.
        /// </summary>
        public void OnReceive(Player player)
        {
            bool canBeExecuted = false;

            foreach (MethodInfo method in Event.GetType().GetMethods())
            {
                if (method.Name != "Execute") continue;

                canBeExecuted = true;

                if (method.GetParameters().Length == Entities.Length + 1)
                {
                    object[] args = new object[Entities.Length + 1];
                    args[0] = player;
                    Array.Copy(Entities.ToArray(), 0, args, 1, Entities.Length);

                    method.Invoke(Event, args);
                    return;
                }
            }

            if (canBeExecuted)
                throw new MissingMethodException(Event.GetType().Name, string.Format("Execute({0})", Entities.Length));
        }

        public override string ToString() => $"SendEventCommand [Event: {Event}, Entities: {String.Join(", ", Entities.Select(x => x.ToString()))}]";
    }
}
