using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Matchmaking
{
    [SerialVersionUID(1499762071035)]
    public class MatchMakingLobbyStartTimeEvent : ECSEvent
    {
        public MatchMakingLobbyStartTimeEvent(TimeSpan startTime)
        {
            StartTime = new TXDate(startTime);
        }
        
        public TXDate StartTime { get; set; }
    }
}