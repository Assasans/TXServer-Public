using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.BattleRewards;
using TXServer.ECSSystem.Components.Item;
using TXServer.ECSSystem.Components.Item.Tank;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.BattleReward;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents.Experience;

namespace TXServer.Core
{
    public class Leveling
    {
        public static void CheckRankUp(Player player)
        {
            List<int> experiencePerRank = new List<int> {0};
            experiencePerRank.AddRange(Config.GetComponent<RanksExperiencesConfigComponent>("ranksconfig")
                .RanksExperiences);

            experiencePerRank.Sort((a, b) => a.CompareTo(b));

            long totalExperience = player.Data.Experience;
            if (player.IsInMatch)
                totalExperience += player.BattlePlayer.MatchPlayer.UserResult.ScoreToExperience -
                                   player.BattlePlayer.MatchPlayer.AlreadyAddedExperience;

            int correctRank = experiencePerRank.IndexOf(experiencePerRank.LastOrDefault(x => x <= totalExperience)) + 1;

            while (player.User.GetComponent<UserRankComponent>().Rank < correctRank)
            {
                player.User.ChangeComponent<UserRankComponent>(component =>
                {
                    component.Rank++;
                    RankUpNotification(component.Rank, player);
                });
            }
        }

        private static void RankUpNotification(int rank, Player player)
        {
            int crystals = rank * 100 - 100;
            int xCrystals = 2 * rank - 2;
            if (xCrystals % 10 != 0) xCrystals = 0;

            player.Data.Crystals += crystals;
            player.Data.XCrystals += xCrystals;

            player.ShareEntities(UserRankRewardNotificationTemplate.CreateEntity(xCrystals, crystals, rank));
            if (player.IsInMatch) player.BattlePlayer.MatchPlayer.RankUp();
        }


        public static void CheckTankRankUp(Entity item, Player player)
        {
            List<int> experiencePerRank = new List<int>(0);
            experiencePerRank.AddRange(Config.GetComponent<UpgradeLevelsComponent>("garage").LevelsExperiences
                .ToList());

            long experience = item.GetComponent<ExperienceItemComponent>().Experience;

            item.RemoveComponent<ExperienceToLevelUpItemComponent>();
            item.AddComponent(new ExperienceToLevelUpItemComponent(experience));
            item.RemoveComponent<UpgradeLevelItemComponent>();
            item.AddComponent(new UpgradeLevelItemComponent(experience));
        }

        public static Entity GetTankRankRewards(Player player)
        {
            Entity notification = LevelUpUnlockBattleRewardTemplate.CreateEntity(new List<Entity>());
            foreach (Entity childItem in player.EntityList.Where(e =>
                e.TemplateAccessor.Template is ChildGraffitiMarketItemTemplate or HullSkinMarketItemTemplate or
                    WeaponSkinMarketItemTemplate))
            {
                if (player.Data.OwnsMarketItem(childItem)) continue;

                Entity parentMarketItem = player.EntityList.Single(e =>
                    e.EntityId == childItem.GetComponent<ParentGroupComponent>().Key);
                if (!player.Data.OwnsMarketItem(parentMarketItem)) continue;

                int neededLevel = Config.GetComponent<MountUpgradeLevelRestrictionComponent>(childItem.TemplateAccessor
                    .ConfigPath).RestrictionValue;
                int itemLevel = player.GetUserItemLevel(parentMarketItem);

                if (itemLevel < neededLevel || neededLevel == 0) continue;

                notification.ChangeComponent<LevelUpUnlockPersonalRewardComponent>(component =>
                    component.Unlocked.Add(childItem));
                ResourceManager.SaveNewMarketItem(player, childItem, 1);
            }

            return notification;
        }
    }
}