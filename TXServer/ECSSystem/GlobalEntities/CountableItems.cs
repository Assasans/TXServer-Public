using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class CountableItems
    {
        public static readonly Entity Crystal = new Entity(947348559);
        public static readonly Entity XCrystal = new Entity(1317350822, new TemplateAccessor(new XCrystalMarketItemTemplate(), "garage/xcrystal"),
            new MarketItemGroupComponent(1317350822));
        public static readonly Entity GoldBonus = new Entity(636909271);
    }
}
