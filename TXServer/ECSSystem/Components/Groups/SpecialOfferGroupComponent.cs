using System;
using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636177020058645390L)]
    public sealed class SpecialOfferGroupComponent : GroupComponent
    {
        public SpecialOfferGroupComponent(Entity key) : base(key)
        {
        }

        public SpecialOfferGroupComponent(long key, Dictionary<string, (double, float)> prices) : base(key)
        {
            Prices = prices;
        }

        public readonly Dictionary<string, (double, float)> Prices = new();

        public readonly Dictionary<string, string> Currencies = new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "DE", "EUR" },
            { "RU", "RUB" },
            { "US", "USD" }
        };
    }
}
