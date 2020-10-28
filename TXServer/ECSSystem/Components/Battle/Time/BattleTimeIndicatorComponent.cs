using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Time
{
    [SerialVersionUID(1447751145383)]
    public class BattleTimeIndicatorComponent : Component
    {
        public BattleTimeIndicatorComponent(string timeText, float progress)
        {
            TimeText = timeText;
            Progress = progress;
        }

        public string TimeText { get; set; }
        
        public float Progress { get; set; }
    }
}