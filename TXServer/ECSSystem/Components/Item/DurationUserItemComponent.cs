using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1513581047619L)]
    public class DurationUserItemComponent : Component
    {
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddHours(6);
    }
}
