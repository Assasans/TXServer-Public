using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class GoldBonuses
    {
        public static readonly Entity PAYMENT_GOODS_GOLDBONUS_1000BOXES = new Entity(-1799018136, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/1000boxes"),
            new SpecialOfferDurationComponent(),
            new SpecialOfferGroupComponent(-1799018136),
            new PackIdComponent(-1799018136),
            new ItemsPackFromConfigComponent(),
            new CrystalsPackComponent(),
            new CountableItemsPackComponent(new Dictionary<Entity, int>() { { CountableItems.GoldBonus, 1000 } }),
            new XCrystalsPackComponent());
        public static readonly Entity PAYMENT_GOODS_GOLDBONUS_800BOXES = new Entity(1242971667, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/800boxes"),
            new SpecialOfferDurationComponent(),
            new SpecialOfferGroupComponent(1242971667),
            new PackIdComponent(1242971667),
            new ItemsPackFromConfigComponent(),
            new CrystalsPackComponent(),
            new CountableItemsPackComponent(new Dictionary<Entity, int>() { { CountableItems.GoldBonus, 800 } }),
            new XCrystalsPackComponent());
        public static readonly Entity PAYMENT_GOODS_GOLDBONUS_80BOXES = new Entity(-467522065, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/80boxes"),
            new SpecialOfferDurationComponent(),
            new SpecialOfferGroupComponent(-467522065),
            new PackIdComponent(-467522065),
            new ItemsPackFromConfigComponent(),
            new CrystalsPackComponent(),
            new CountableItemsPackComponent(new Dictionary<Entity, int>() { { CountableItems.GoldBonus, 80 } }),
            new XCrystalsPackComponent());
        public static readonly Entity PAYMENT_GOODS_GOLDBONUS_8BOXES = new Entity(724226707, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/8boxes"),
            new SpecialOfferDurationComponent(),
            new SpecialOfferGroupComponent(724226707),
            new PackIdComponent(724226707),
            new ItemsPackFromConfigComponent(),
            new CrystalsPackComponent(),
            new CountableItemsPackComponent(new Dictionary<Entity, int>() { { CountableItems.GoldBonus, 8 } }),
            new XCrystalsPackComponent());
    }
}
