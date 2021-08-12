using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(636329559762986136L)]
    public class UpgradeModuleWithCrystalsEvent : ECSEvent
    {
        public static void Execute(Player player, Entity marketItem) => TryUpgradeModule(player, marketItem);

        protected static void TryUpgradeModule(Player player, Entity marketItem, bool xCrystals = false)
        {
            long id = marketItem.GetComponent<ParentGroupComponent>().Key;
            (int level, int cards) infos = player.Data.Modules[id];
            Entity moduleItem = ResourceManager.GetModuleUserItem(player, id);

            ModulePrice modulePrice =
                moduleItem.GetComponent<ModuleCardsCompositionComponent>().AllPrices[infos.level + 1];
            int price = xCrystals ? modulePrice.XCrystals : modulePrice.Crystals;

            // todo: check if enough blueprints

            if (xCrystals && player.Data.XCrystals >= price)
                player.Data.XCrystals -= price;
            else if (!xCrystals && player.Data.Crystals >= price)
                player.Data.Crystals -= price;
            else
                return;

            player.Data.UpgradeModule(marketItem);
        }
    }
}
