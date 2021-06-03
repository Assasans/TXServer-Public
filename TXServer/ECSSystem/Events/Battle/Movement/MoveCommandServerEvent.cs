using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Movement
{
    [SerialVersionUID(-4956413533647444536L)]
    public class MoveCommandServerEvent : ECSEvent, IRemoteEvent
    {
        public MoveCommand MoveCommand { get; set; }
    }
}
