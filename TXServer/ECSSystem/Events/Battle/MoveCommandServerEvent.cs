using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-4956413533647444536L)]
    public class MoveCommandServerEvent : ECSEvent
    {
        public MoveCommandServerEvent(MoveCommand moveCommand)
        {
            MoveCommand = moveCommand;
        }

        public MoveCommand MoveCommand { get; set; }
    }
}