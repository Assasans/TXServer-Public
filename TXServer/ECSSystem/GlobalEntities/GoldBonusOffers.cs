using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class GoldBonusOffers
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity _400boxes { get; } = new Entity(-1799018136, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/1000boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(-1799018136, new Dictionary<string, (double, float)>
                {
                    { "EUR", (73.90, 0) },
                    { "USD", (27.99, 0) },
                    { "RUB", (3990.00, 0) }
                }),
                new PackIdComponent(-1799018136),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Goldbonus, 400 }
                }),
                new XCrystalsPackComponent());
            public Entity _80boxes { get; } = new Entity(1242971667, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/800boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(1242971667, new Dictionary<string, (double, float)>
                {
                    { "EUR", (69.90, 0) },
                    { "USD", (7.99, 0) },
                    { "RUB", (3749.00, 0) }
                }),
                new PackIdComponent(1242971667),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Goldbonus, 80 }
                }),
                new XCrystalsPackComponent());
            public Entity _50boxes { get; } = new Entity(-467522065, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/80boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(-467522065, new Dictionary<string, (double, float)>
                {
                    { "EUR", (9.99, 0) },
                    { "USD", (5.99, 0) },
                    { "RUB", (499.00, 0) }
                }),
                new PackIdComponent(-467522065),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Goldbonus, 50 }
                }),
                new XCrystalsPackComponent());
            public Entity _8boxes { get; } = new Entity(724226707, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/8boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(724226707, new Dictionary<string, (double, float)>
                {
                    { "EUR", (1.49, 0) },
                    { "USD", (1.49, 0) },
                    { "RUB", (79.00, 0) }
                }),
                new PackIdComponent(724226707),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Goldbonus, 8 }
                }),
                new XCrystalsPackComponent());
        }
    }
}
