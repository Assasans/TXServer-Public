using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.VisualScore
{
    [SerialVersionUID(1511432397963L)]
    public class VisualScoreFlagDeliverEvent : VisualScoreEvent, ECSEvent
    {
        public VisualScoreFlagDeliverEvent(int score)
        {
            Score = score;
        }
    }
}