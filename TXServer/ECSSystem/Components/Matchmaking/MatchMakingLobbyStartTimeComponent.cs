using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1496833452921)]
    public class MatchMakingLobbyStartTimeComponent : Component
    {
        public MatchMakingLobbyStartTimeComponent(TimeSpan span)
        {
            StartTime = new TXDate(span);
        }
        
        public TXDate StartTime { get; set; }
    }
}