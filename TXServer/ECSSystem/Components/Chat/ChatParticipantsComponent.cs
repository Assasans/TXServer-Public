using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636437655901996504L)]
    public class ChatParticipantsComponent : Component
    {
        public ChatParticipantsComponent(List<Entity> users)
        {
            Users = users ?? throw new ArgumentNullException(nameof(users));
        }

        public ChatParticipantsComponent(params Entity[] users)
        {
            Users = users.ToList();
        }

        public List<Entity> Users { get; set; }
    }
}
