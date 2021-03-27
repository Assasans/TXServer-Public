using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.VisualScore
{
    [SerialVersionUID(1511432353980L)]
    public class VisualScoreAssistEvent : VisualScoreEvent, ECSEvent
    {
        public VisualScoreAssistEvent(string targetUid, int percent, int score)
        {
            TargetUid = targetUid;
            Percent = percent;
            Score = score;
        }

        public string TargetUid { get; set; }
        public int Percent { get; set; }
    }
}