﻿using System.Collections.Generic;
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
            public Entity _1000boxes { get; } = new Entity(-1799018136, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/1000boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(-1799018136),
                new PackIdComponent(-1799018136),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Goldbonus, 1000 }
                }),
                new XCrystalsPackComponent());
            public Entity _800boxes { get; } = new Entity(1242971667, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/800boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(1242971667),
                new PackIdComponent(1242971667),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Goldbonus, 800 }
                }),
                new XCrystalsPackComponent());
            public Entity _80boxes { get; } = new Entity(-467522065, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/80boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(-467522065),
                new PackIdComponent(-467522065),
                new ItemsPackFromConfigComponent(),
                new CrystalsPackComponent(),
                new CountableItemsPackComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Goldbonus, 80 }
                }),
                new XCrystalsPackComponent());
            public Entity _8boxes { get; } = new Entity(724226707, new TemplateAccessor(new GoldBonusOfferTemplate(), "payment/goods/goldbonus/8boxes"),
                new SpecialOfferDurationComponent(),
                new SpecialOfferGroupComponent(724226707),
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
