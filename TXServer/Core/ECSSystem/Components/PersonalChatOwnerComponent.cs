using System.Collections.Generic;

namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1513067769958)]
    public class PersonalChatOwnerComponent : Component
    {
        public List<long> ChatsIs { get; set; } = new List<long>();
    }
}
