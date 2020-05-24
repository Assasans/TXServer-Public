using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1513067769958)]
    public class PersonalChatOwnerComponent : Component
    {
        public List<Entity> ChatsIs { get; set; } = new List<Entity>();
    }
}
