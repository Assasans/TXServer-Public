using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.VisualScore
{
    [SerialVersionUID(1511846568255L)]
    public class VisualScoreHealEvent : VisualScoreEvent, ECSEvent
    {
        public VisualScoreHealEvent(int score)
        {
            Score = score;
        }
    }
}