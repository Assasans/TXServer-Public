using System.Reflection;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Offer;
using TXServer.ECSSystem.Components.Payment;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class PersonalSpecialOffers
    {
        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.Components.Add(new UserGroupComponent(user));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity _1000boxes { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(GoldBonusOffers.GlobalItems._1000boxes),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
            public Entity _800boxes { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(GoldBonusOffers.GlobalItems._800boxes),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
            public Entity _80boxes { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(GoldBonusOffers.GlobalItems._80boxes),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
            public Entity _8boxes { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(GoldBonusOffers.GlobalItems._8boxes),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
            public Entity _3day { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(PremiumOffers.GlobalItems._3day),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
            public Entity _7day { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(PremiumOffers.GlobalItems._7day),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
            public Entity _7dayx { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(PremiumOffers.GlobalItems._7dayx),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
            public Entity _30dayx { get; } = new Entity(new TemplateAccessor(new PersonalSpecialOfferPropertiesTemplate(), ""),
                new SpecialOfferGroupComponent(PremiumOffers.GlobalItems._30dayx),
                new PackIdComponent(0),
                new DiscountComponent(0),
                new SpecialOfferRemainingTimeComponent(12345),
                new PaymentIntentComponent(),
                new SpecialOfferVisibleComponent());
        }
    }
}
