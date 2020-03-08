using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Bits;
using TXServer.Core.ECSSystem;
using TXServer.Core.ECSSystem.Events;

namespace TXServer.Core.Commands
{
    public class SendEventCommand : Command
    {
        public SendEventCommand() { }

        public SendEventCommand(ECSEvent Event, params Entity[] Entities)
        {
            if (Entities == null)
            {
                throw new ArgumentNullException("Не указаны сущности.");
            }
            this.Event = Event;
            this.Entities = Entities.ToList();
        }

        public override void BeforeWrap()
        {
        }

        public override void AfterUnwrap()
        {
            foreach (Entity entity in Entities)
            {
                Event.Execute(entity);
            }
        }

        [Protocol] public ECSEvent Event { get; set; }
        [Protocol] public List<Entity> Entities { get; set; } = new List<Entity>();
    }
}
