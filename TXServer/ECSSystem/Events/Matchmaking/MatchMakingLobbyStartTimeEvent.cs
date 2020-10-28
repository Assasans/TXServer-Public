using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Matchmaking
{
    [SerialVersionUID(1499762071035)]
    public class MatchMakingLobbyStartTimeEvent : ECSEvent
    {
        public MatchMakingLobbyStartTimeEvent(DateTimeOffset startTime)
        {
            StartTime = startTime.ToUnixTimeMilliseconds();
        }
        
        public long StartTime { get; set; }
    }
}