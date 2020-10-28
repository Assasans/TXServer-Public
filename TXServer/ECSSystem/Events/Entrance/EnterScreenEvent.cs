using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1453867134827)]
    public class EnterScreenEvent : ECSEvent
    {
        public string Screen { get; set; }

        public void Execute(Player player, Entity entity)
        {
            Console.WriteLine($"User is entering screen {Screen}");
        }
    }
}
