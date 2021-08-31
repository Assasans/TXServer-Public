using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Tutorial
{
    [SerialVersionUID(1505212007257L)]
    public class ApplyTutorialIdEvent : ECSEvent
    {
        public void Execute(Player player, Entity session) => player.Data.AddCompletedTutorial(Id);

        public long Id { get; set; }
    }
}
