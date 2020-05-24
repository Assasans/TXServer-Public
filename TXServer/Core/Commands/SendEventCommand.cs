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
            bool executable = false;

            MethodInfo[] methods = Event.GetType().GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.Name == "Execute")
                {
                    executable = true;
                    if (method.GetParameters().Length == Entities.Count)
                    {
                        method.Invoke(Event, Entities.ToArray());
                        return;
                    }
                }
            }
            if (executable)
                throw new MissingMethodException(Event.GetType().Name, string.Format("Execute({0})", Entities.Count));
        }

        [ProtocolFixed] public ECSEvent Event { get; set; }
        [ProtocolFixed] public List<Entity> Entities { get; set; }
    }
}
