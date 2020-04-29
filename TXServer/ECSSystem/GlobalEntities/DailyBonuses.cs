using System.Reflection;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class DailyBonuses
    {
        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.Components.Add(new UserGroupComponent(user));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Energy { get; } = new Entity(new TemplateAccessor(new EnergyDailyBonusTemplate(), "bonus/energy/daily"),
                new ExpireDateComponent());
            public Entity Money { get; } = new Entity(new TemplateAccessor(new MoneyDailyBonusTemplate(), "bonus/money/daily"),
                new ExpireDateComponent());
            public Entity Quest { get; } = new Entity(new TemplateAccessor(new QuestDailyBonusTemplate(), "bonus/questexchange/daily"),
                new ExpireDateComponent());
        }
    }
}
