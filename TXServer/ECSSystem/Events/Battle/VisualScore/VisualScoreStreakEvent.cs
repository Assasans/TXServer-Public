using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.VisualScore
{
    [SerialVersionUID(1512478367453L)]
    public class VisualScoreStreakEvent : VisualScoreEvent, ECSEvent
    {
        public VisualScoreStreakEvent(int score)
        {
            Score = score;
        }
    }
}