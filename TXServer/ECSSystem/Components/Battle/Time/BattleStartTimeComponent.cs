using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Time
{
    [SerialVersionUID(1436521738148L)]
    public class BattleStartTimeComponent : Component
    {
        public BattleStartTimeComponent(DateTimeOffset roundStartTime)
        {
            RoundStartTime = roundStartTime.ToUnixTimeMilliseconds();
        }

        public long RoundStartTime { get; set; }
    }
}