using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Tutorial
{
    [SerialVersionUID(1506070003266L)]
    public class TutorialActionEvent : ECSEvent
    {
        public void Execute(Player player, Entity session)
        {
            switch (TutorialId, StepId, Action)
            {
                case (419965140, -578695959, TutorialAction.END):
                    if (!player.Data.OwnsMarketItem(Graffiti.GlobalItems.Armsrace))
                        player.SaveNewMarketItem(Graffiti.GlobalItems.Armsrace);
                    break;
            }
        }

        public long TutorialId { get; set; }
        public long StepId { get; set; }
        public TutorialAction Action { get; set; }
    }
}
