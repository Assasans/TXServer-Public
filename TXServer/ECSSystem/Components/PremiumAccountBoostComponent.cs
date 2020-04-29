using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1513252416040L)]
    public class PremiumAccountBoostComponent : Component
    {
        public TXDate EndDate { get; set; } = new TXDate(new TimeSpan(6, 0, 0));
    }
}
