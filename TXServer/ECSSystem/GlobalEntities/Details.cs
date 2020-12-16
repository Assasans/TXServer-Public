using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Details
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Entity.GenerateId();

                item.TemplateAccessor.Template = new DetailUserItemTemplate();

                item.Components.Add(new UserGroupComponent(user.EntityId));
                item.Components.Add(new UserItemCounterComponent(0));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Gold { get; } = new Entity(215382134, new TemplateAccessor(new DetailMarketItemTemplate(), "garage/detail/gold"),
                 new MarketItemGroupComponent(215382134));
            public Entity Rubber { get; } = new Entity(1143965766, new TemplateAccessor(new DetailMarketItemTemplate(), "garage/detail/rubber"),
                new MarketItemGroupComponent(1143965766));
            public Entity Xt { get; } = new Entity(-53406574, new TemplateAccessor(new DetailMarketItemTemplate(), "garage/detail/xt"),
                new MarketItemGroupComponent(-53406574));
        }
    }
}
