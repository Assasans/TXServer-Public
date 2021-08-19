using System.Collections.Generic;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class XCrystalsPacks
    {
        public static Items GlobalItems { get; } = new();

        public class Items : ItemList
        {
            public Entity _125crystals { get; } = new Entity(-122786427, new TemplateAccessor(new XCrystalsPackTemplate(), ""),
                new PackIdComponent(-122786427),
                new SpecialOfferGroupComponent(-122786427, new Dictionary<string, (double, float)>
                {
                    { "USD", (2, 0) },
                    { "RUB", (100, 0) }
                }),
                new XCrystalsPackComponent()
                {
                    Amount = 125,
                    Bonus = 0
                });
            public Entity _250crystals { get; } = new Entity(-122786426, new TemplateAccessor(new XCrystalsPackTemplate(), ""),
                new PackIdComponent(-122786426),
                new SpecialOfferGroupComponent(-122786426, new Dictionary<string, (double, float)>
                {
                    { "USD", (3.99, 0) },
                    { "RUB", (199, 0) }
                }),
                new XCrystalsPackComponent()
                {
                    Amount = 250,
                    Bonus = 75
                });
            public Entity _750crystals { get; } = new Entity(-122786425, new TemplateAccessor(new XCrystalsPackTemplate(), ""),
                new PackIdComponent(-122786425),
                new SpecialOfferGroupComponent(-122786425, new Dictionary<string, (double, float)>
                {
                    { "USD", (11.99, 0) },
                    { "RUB", (599, 0) }
                }),
                new XCrystalsPackComponent()
                {
                    Amount = 750,
                    Bonus = 300
                });
            public Entity _1500crystals { get; } = new Entity(-122786424, new TemplateAccessor(new XCrystalsPackTemplate(), ""),
                new PackIdComponent(-122786424),
                new SpecialOfferGroupComponent(-122786424, new Dictionary<string, (double, float)>
                {
                    { "USD", (23.99, 0) },
                    { "RUB", (1199, 0) }
                }),
                new XCrystalsPackComponent()
                {
                    Amount = 1500,
                    Bonus = 750
                });
            public Entity _3750crystals { get; } = new Entity(-122786423, new TemplateAccessor(new XCrystalsPackTemplate(), ""),
                new PackIdComponent(-122786423),
                new SpecialOfferGroupComponent(-122786423, new Dictionary<string, (double, float)>
                {
                    { "USD", (59.99, 0) },
                    { "RUB", (2999, 0) }
                }),
                new XCrystalsPackComponent()
                {
                    Amount = 3750,
                    Bonus = 2000
                });
            public Entity _6250crystals { get; } = new Entity(-122786422, new TemplateAccessor(new XCrystalsPackTemplate(), ""),
                new PackIdComponent(-122786422),
                new SpecialOfferGroupComponent(-122786422, new Dictionary<string, (double, float)>
                {
                    { "USD", (99.99, 0) },
                    { "RUB", (4999, 0) }
                }),
                new XCrystalsPackComponent()
                {
                    Amount = 6250,
                    Bonus = 3750
                });
        }
    }
}
