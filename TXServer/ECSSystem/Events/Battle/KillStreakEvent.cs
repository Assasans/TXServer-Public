using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1491556721814L)]
    public class KillStreakEvent : ECSEvent
    {
        public KillStreakEvent(int score)
        {
            Score = score;
        }
        public int Score { get; set; }
    }
}