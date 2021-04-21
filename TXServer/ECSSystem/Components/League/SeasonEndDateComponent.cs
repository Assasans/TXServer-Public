using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1505728594733L)]
    public class SeasonEndDateComponent : Component
    {
        [OptionalMapped]
        public DateTime EndDate { get; set; } = DateTime.Now.AddHours(6);
    }
}
