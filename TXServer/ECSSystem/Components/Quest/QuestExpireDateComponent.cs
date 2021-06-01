using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1476707093577L)]
    public class QuestExpireDateComponent : Component
    {
        public DateTime Date { get; set; } = DateTime.UtcNow.AddHours(6);
    }
}
