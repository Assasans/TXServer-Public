using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1481177510866L)]
    public class NewItemNotificationComponent : Component
    {
        public NewItemNotificationComponent(Entity item, int amount)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Amount = amount;
        }

        public Entity Item { get; set; }

        public int Amount { get; set; }
    }
}
