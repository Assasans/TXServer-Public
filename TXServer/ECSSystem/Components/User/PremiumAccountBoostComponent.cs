using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1513252416040L)]
    public class PremiumAccountBoostComponent : Component
    {
        public PremiumAccountBoostComponent(DateTime endDate) => EndDate = endDate;

        public DateTime EndDate { get; set; }
    }
}
