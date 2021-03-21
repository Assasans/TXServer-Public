using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Score
{
    [SerialVersionUID(1439453338183L)]
    public class RoundUserStatisticsUpdatedEvent : ECSEvent
    {
    }
    [SerialVersionUID(1463648611538L)]
    public class SetScoreTablePositionEvent : ECSEvent
    {
        public int Position { get; set; }

        public SetScoreTablePositionEvent(int position)
        {
            Position = position;
        }
    }
}