using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Events.Item;
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
                case (-719658163, 1331311973, TutorialAction.START):
                    // todo: check if repair kit is owned and add if not (level 1, 0 blueprints)
                    // unmount all modules
                    foreach (Entity slot in player.CurrentPreset.Modules.Keys.ToList())
                    {
                        slot.TryRemoveComponent<ModuleGroupComponent>();
                        player.CurrentPreset.Modules[slot]?.TryRemoveComponent<MountedItemComponent>();
                        player.CurrentPreset.Modules[slot] = null;
                    }

                    // mount repair kit
                    new ModuleMountEvent().Execute(player,
                        player.EntityList.Single(e =>
                            e.TemplateAccessor.Template.GetType() == typeof(ModuleUserItemTemplate) &&
                            e.TemplateAccessor.ConfigPath.Split('/').Last() == "repairkit"),
                        player.CurrentPreset.Modules.Keys.ToList()[5]);

                    player.SendEvent(new TutorialIdResultEvent(StepId, true, true), player.ClientSession);

                    break;
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
