using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.ECSSystem;
using TXServer.Core.ECSSystem.Events;

namespace TXServer.Core.Commands
{
    public class SendEventCommand : Command
    {
        public SendEventCommand(ECSEvent Event, params Entity[] Entities)
        {
            if (Entities == null)
            {
                throw new ArgumentNullException("Не указаны сущности.");
            }
            this.Event = Event;
            this.Entities = Entities.ToList();
        }

        public override void OnSend()
        {
        }

        public override void OnReceive()
        {
            foreach (Entity entity in Entities)
            {
                Event.Execute(entity);
            }
        }

        [ProtocolFixed] public ECSEvent Event { get; set; }
        [ProtocolFixed] public List<Entity> Entities { get; set; }
    }
}
