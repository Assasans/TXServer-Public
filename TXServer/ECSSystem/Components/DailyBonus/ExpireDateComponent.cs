using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1504598280798L)]
    public class ExpireDateComponent : Component
    {
        public DateTime Date { get; set; } = DateTime.Now.AddHours(6);
    }
}
