using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1513252653655L)]
    public class PremiumAccountQuestComponent : Component
    {
        public TXDate EndDate { get; set; } = new TXDate(new TimeSpan(6, 0, 0));
    }
}
