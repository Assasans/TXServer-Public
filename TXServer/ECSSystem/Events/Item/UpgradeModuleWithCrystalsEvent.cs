using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
using ModuleInfo = TXServer.Core.Battles.Effect.ModuleInfo;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(636329559762986136L)]
    public class UpgradeModuleWithCrystalsEvent : ECSEvent
    {
        public static void Execute(Player player, Entity marketItem) => TryUpgradeModule(player, marketItem);

        protected static void TryUpgradeModule(Player player, Entity marketItem, bool xCrystals = false)
        {
            long id = marketItem.GetComponent<ParentGroupComponent>().Key;
            ModuleInfo info = player.Data.Modules[id];
            Entity moduleItem = ResourceManager.GetModuleUserItem(player, id);

            // check if already max level
            ModuleCardsCompositionComponent compositionComponent =
                moduleItem.GetComponent<ModuleCardsCompositionComponent>();
            if (compositionComponent.AllPrices.Count < info.Level + 2) return;

            ModulePrice modulePrice = compositionComponent.AllPrices[info.Level + 1];
            int price = xCrystals ? modulePrice.XCrystals : modulePrice.Crystals;

            // check if player has enough blueprints to upgrade
            if (player.Data.Modules.TryGetValue(id, out ModuleInfo blueprint) &&
                blueprint.Cards < modulePrice.Cards) return;

            switch (xCrystals)
            {
                case true when player.Data.XCrystals >= price:
                    player.Data.XCrystals -= price;
                    break;
                case false when player.Data.Crystals >= price:
                    player.Data.Crystals -= price;
                    break;
                default:
                    return;
            }

            player.Data.UpgradeModule(marketItem);
        }
    }
}
