using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class PremiumOffers
    {
        public static Items GlobalItems { get; } = new();

        public class Items : ItemList
        {
            public Entity _3day { get; } = new Entity(-763254490, new TemplateAccessor(new PremiumOfferTemplate(), "payment/goods/premium/3day"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(-763254490, new Dictionary<string, (double, float)>
                {
                    { "USD", (1.99, 0) },
                    { "RUB", (199, 0) },
                }),
                new PackIdComponent(-763254490),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Premiumboost, 3 },
                    { ExtraItems.GlobalItems.Premiumquest, 3 }
                }),
                new XCrystalsPackComponent());
            public Entity _7day { get; } = new Entity(-1186619009, new TemplateAccessor(new PremiumOfferTemplate(), "payment/goods/premium/7day"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(-1186619009, new Dictionary<string, (double, float)>
                {
                    { "USD", (3.99, 0) },
                    { "RUB", (599, 0) },
                }),
                new PackIdComponent(-1186619009),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Premiumboost, 7 },
                }),
                new XCrystalsPackComponent());
            public Entity _7dayx { get; } = new Entity(1869516473, new TemplateAccessor(new PremiumOfferTemplate(), "payment/goods/premium/7dayX"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(1869516473, new Dictionary<string, (double, float)>
                {
                    { "USD", (3.99, 0) },
                }),
                new PackIdComponent(1869516473),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Premiumboost, 7 },
                    { ExtraItems.GlobalItems.Premiumquest, 7 }
                }),
                new XCrystalsPackComponent());
            public Entity _30dayx { get; } = new Entity(1957963539, new TemplateAccessor(new PremiumOfferTemplate(), "payment/goods/premium/30dayX"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(1957963539, new Dictionary<string, (double, float)>
                {
                    { "USD", (11.99, 0) }
                }),
                new PackIdComponent(1957963539),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Premiumboost, 30 },
                    { ExtraItems.GlobalItems.Premiumquest, 30 }
                }),
                new XCrystalsPackComponent());
        }
    }
}
