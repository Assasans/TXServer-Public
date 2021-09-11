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
                case (419965140, -578695959, TutorialAction.END):
                    if (!player.Data.OwnsMarketItem(Graffiti.GlobalItems.Armsrace))
                        player.SaveNewMarketItem(Graffiti.GlobalItems.Armsrace);
                    break;
                case (-1423861367, 1325524063, TutorialAction.START):
                    player.SaveNewMarketItem(Containers.GlobalItems.Tutorialbronze1);
                    SendResult(player);
                    break;
                case (-2063696990, 723559096, TutorialAction.START):
                    // turretChangeTutorial: step 7 ["first turret is on us"]
                    player.Data.Crystals += 200;
                    SendResult(player);
                    break;
                case (-719658163, 1331311973, TutorialAction.START):
                    // firstEntranceTutorial: step 4 [mount repair kit]
                    // todo: check if repair kit is owned and add if not add it (level 1, 0 blueprints)
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
                        player.CurrentPreset.Modules.Keys.ToList()[3]);

                    SendResult(player);
                    break;
                case (-719658163, 1331311974, TutorialAction.END) or (-2063696990, 723559099, TutorialAction.END):
                    // firstEntranceTutorial/turretChangeTutorial: step 5/11 [enter training battle]
                    Core.Battles.Matchmaking.MatchMaking.EnterMatchMaking(player,
                        MatchmakingModes.GlobalItems.Training);
                    break;
            }
        }

        private void SendResult(Player player) =>
            player.SendEvent(new TutorialIdResultEvent(StepId, true, true), player.ClientSession);

        public long TutorialId { get; set; }
        public long StepId { get; set; }
        public TutorialAction Action { get; set; }
    }
}
