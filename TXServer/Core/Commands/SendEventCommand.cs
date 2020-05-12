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

        public override void OnSend()
        {
        }

        /// <summary>
        /// Detects event method with same number of Entity parameters and calls it.
        /// </summary>
        public override void OnReceive()
        {
            Type[] methodArgs = new Type[Entities.Count];
            for (int i = 0; i < methodArgs.Length; i++)
            {
                methodArgs[i] = typeof(Entity);
            }

            MethodInfo method = Event.GetType().GetMethod("Execute");
            if (method != null && method.GetParameters().Length != Entities.Count) throw new MissingMethodException(Event.GetType().Name, "Execute");
            method?.Invoke(Event, Entities.ToArray());
        }

        [ProtocolFixed] public ECSEvent Event { get; set; }
        [ProtocolFixed] public List<Entity> Entities { get; set; }
    }
}
