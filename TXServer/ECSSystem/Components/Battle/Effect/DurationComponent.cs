using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(5192591761194414739L)]
    public class DurationComponent : Component
    {
        public DateTime StartedTime { get; set; }
    }
}