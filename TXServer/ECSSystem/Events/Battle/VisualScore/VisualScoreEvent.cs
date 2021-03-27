using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.VisualScore
{
    public class VisualScoreEvent : ECSEvent
    {
        public int Score { get; set; }
    }
}