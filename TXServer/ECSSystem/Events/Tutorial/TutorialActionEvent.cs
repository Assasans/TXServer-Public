using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Tutorial
{
    [SerialVersionUID(1506070003266L)]
    public class TutorialActionEvent : ECSEvent
    {
        public long TutorialId { get; set; }
        public long StepId { get; set; }
        public TutorialAction Action { get; set; }
    }
}
