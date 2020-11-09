using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Commands
{
    [CommandCode(1)]
    public class SendEventCommand : Command
    {
        public SendEventCommand(ECSEvent Event, params Entity[] Entities)
        {
            _ = Entities ?? throw new ArgumentNullException(nameof(Entities));

            this.Event = Event;
            this.Entities = Entities.ToList();
        }

        public override void OnSend(Player player)
        {
        }

        /// <summary>
        /// Detects event method with same number of Entity parameters and calls it.
        /// </summary>
        public override void OnReceive(Player player)
        {
            bool canBeExecuted = false;
            MethodInfo[] methods = Event.GetType().GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.Name != "Execute") continue;

                canBeExecuted = true;
                ParameterInfo[] parameters = method.GetParameters();

                // Player can be passed as first parameter before entities
                if (parameters.Length == Entities.Count + 1 && parameters[0].ParameterType == typeof(Player))
                {
                    object[] args = new object[Entities.Count + 1];
                    args[0] = player;
                    Array.Copy(Entities.ToArray(), 0, args, 1, Entities.Count);

                    method.Invoke(Event, args);
                    return;
                }

                // Only entities as parameters
                if (parameters.Length == Entities.Count && parameters[0].ParameterType != typeof(Player))
                {
                    method.Invoke(Event, Entities.ToArray());
                    return;
                }
            }
            if (canBeExecuted)
                throw new MissingMethodException(Event.GetType().Name, string.Format("Execute({0})", Entities.Count));
        }

        [ProtocolFixed] public ECSEvent Event { get; set; }
        [ProtocolFixed] public List<Entity> Entities { get; set; }
    }
}
