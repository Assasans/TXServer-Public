using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.VisualScore
{
    [SerialVersionUID(1511432334883L)]
    public class VisualScoreKillEvent : VisualScoreEvent, ECSEvent
    {
        public VisualScoreKillEvent(string targetUid, int targetRank, int score)
        {
            TargetUid = targetUid;
            TargetRank = targetRank;
            Score = score;
        }

        public string TargetUid { get; set; }
        public int TargetRank { get; set; }
    }
}