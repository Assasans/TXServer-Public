using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Tutorial
{
    [SerialVersionUID(1505212162608L)]
    public class TutorialIdResultEvent : ECSEvent
    {
        public TutorialIdResultEvent(long id, bool actionExecuted, bool actionSuccess)
        {
            Id = id;
            ActionExecuted = actionExecuted;
            ActionSuccess = actionSuccess;
        }

        public long Id { get; set; }
        public bool ActionExecuted { get; set; }
        public bool ActionSuccess { get; set; }
    }
}
