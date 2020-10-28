using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Bonus
{
    [SerialVersionUID(-7944772313373733709)]
    public class BonusDropTimeComponent : Component
    {
        public BonusDropTimeComponent(DateTimeOffset dropTime)
        {
            DropTime = dropTime.ToUnixTimeMilliseconds();
        }
        
        public long DropTime { get; set; }
    }
}