using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Bonus
{
    [SerialVersionUID(-7944772313373733709)]
    public class BonusDropTimeComponent : Component
    {
        public BonusDropTimeComponent(DateTime dropTime)
        {
            DropTime = dropTime;
        }

        public DateTime DropTime { get; set; }
    }
}