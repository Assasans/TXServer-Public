using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.VisualScore
{
    [SerialVersionUID(1511432446237L)]
    public class VisualScoreFlagReturnEvent : VisualScoreEvent, ECSEvent
    {
        public VisualScoreFlagReturnEvent(int score)
        {
            Score = score;
        }
    }
}