using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1505728594733L)]
    public class SeasonEndDateComponent : Component
    {
        [OptionalMapped]
        public TXDate EndDate { get; set; } = new TXDate(new TimeSpan(6, 0, 0));
    }
}
