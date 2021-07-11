using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Item.Module;
using TXServer.ECSSystem.EntityTemplates.Item.Module;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(636329559762986136L)]
    public class UpgradeModuleWithCrystalsEvent : ECSEvent
    {
        public static void Execute(Player player, Entity marketItem)
        {
            long id = marketItem.GetComponent<ParentGroupComponent>().Key;
            (int level, int cards) infos = player.Data.Modules[id];
            Entity moduleItem = player.EntityList.Single(e => e.TemplateAccessor.Template is
                ModuleUserItemTemplate && e.GetComponent<ParentGroupComponent>().Key == id);

            player.Data.Crystals -= moduleItem.GetComponent<ModuleCardsCompositionComponent>()
                .AllPrices[infos.level + 1].Crystals;

            player.Data.UpgradeModule(marketItem);
        }
    }
}