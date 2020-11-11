using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-2650671245931951659L)]
    public class MuzzlePointSwitchEvent : ECSEvent
    {
        public int Index { get; set; }

        public MuzzlePointSwitchEvent()
        {
        }

        public MuzzlePointSwitchEvent(int index)
        {
            this.Index = index;
        }
    }
}