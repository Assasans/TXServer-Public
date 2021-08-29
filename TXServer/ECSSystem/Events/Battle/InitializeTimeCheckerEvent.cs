using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1461735527769)]
    public class InitializeTimeCheckerEvent : ECSEvent
    {
        public void Execute(Player player, Entity incarnation)
        {
        }
    }
}
