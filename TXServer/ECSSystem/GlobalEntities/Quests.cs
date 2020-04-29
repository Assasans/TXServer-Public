using System.Collections.Generic;
using System.Reflection;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Quests
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
            public Entity Frags { get; } = new Entity(new TemplateAccessor(new FragQuestTemplate(), "quests/daily/frags"),
                new FragQuestComponent(),
                new QuestRewardComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Crystal, 100 }
                }),
                new SlotIndexComponent(0),
                new QuestConditionComponent(),
                new QuestProgressComponent(),
                new QuestExpireDateComponent(),
                new QuestRarityComponent(),
                new QuestComponent());
            public Entity Battle { get; } = new Entity(new TemplateAccessor(new BattleCountQuestTemplate(), "quests/daily/battle"),
                new FragQuestComponent(),
                new QuestRewardComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Crystal, 100 }
                }),
                new SlotIndexComponent(1),
                new QuestConditionComponent(),
                new QuestProgressComponent(),
                new QuestExpireDateComponent(),
                new QuestRarityComponent(),
                new QuestComponent());
            public Entity Score { get; } = new Entity(new TemplateAccessor(new ScoreQuestTemplate(), "quests/daily/scores"),
                new FragQuestComponent(),
                new QuestRewardComponent(new Dictionary<Entity, int>
                {
                    { ExtraItems.GlobalItems.Crystal, 100 }
                }),
                new SlotIndexComponent(2),
                new QuestConditionComponent(),
                new QuestProgressComponent(),
                new QuestExpireDateComponent(),
                new QuestRarityComponent(),
                new QuestComponent());
        }
    }
}
