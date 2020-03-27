﻿using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Protocol;
using TXServer.ECSSystem;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events;

namespace TXServer.Core.Commands
{
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
