using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1496833452921)]
    public class MatchMakingLobbyStartTimeComponent : Component
    {
        public MatchMakingLobbyStartTimeComponent(DateTimeOffset startTime)
        {
            StartTime = startTime.ToUnixTimeMilliseconds();
        }
        
        public long StartTime { get; set; }
    }
}