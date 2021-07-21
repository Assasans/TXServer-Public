using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.DailyBonus;
using TXServer.ECSSystem.Components.User;
using TXServer.ECSSystem.Components.User.Tutorial;

namespace TXServer.ECSSystem.EntityTemplates.User
{
    [SerialVersionUID(1433752208915)]
    public class UserTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Player player)
        {
            Entity user = new(new TemplateAccessor(new UserTemplate(), ""),
                new UserComponent(),
                new UserOnlineComponent(),

                new UserUidComponent(player.Data.Username),
                new UserCountryComponent(player.Data.CountryCode),
                new UserAvatarComponent("8b74e6a3-849d-4a8d-a20e-be3c142fd5e8"),
                new RegistrationDateComponent(),

                new UserMoneyComponent(player.Data.Crystals),
                new UserXCrystalsComponent(player.Data.XCrystals),
                new UserRankComponent(1),
                new UserExperienceComponent(player.Data.Experience),
                new UserReputationComponent(player.Data.Reputation),

                new TutorialCompleteIdsComponent(player.Data.CompletedTutorialIds, player),

                new FractionUserScoreComponent(player.Data.FractionUserScore),

                new UserStatisticsComponent(),
                new FavoriteEquipmentStatisticsComponent(),
                new KillsEquipmentStatisticsComponent(),
                new BattleLeaveCounterComponent(0, 0),

                new PersonalChatOwnerComponent(),

                new LeagueGroupComponent(player.Data.League),
                new GameplayChestScoreComponent(player.Data.LeagueChestScore),

                new BlackListComponent(),

                new UserDailyBonusInitializedComponent(),
                new UserDailyBonusCycleComponent(player.Data.DailyBonusCycle),
                new UserDailyBonusReceivedRewardsComponent(),
                new UserDailyBonusZoneComponent(player.Data.DailyBonusZone),
                new UserDailyBonusNextReceivingDateComponent(player.Data.DailyBonusNextReceiveDate),

                new QuestReadyComponent(),
                new UserPublisherComponent(),
                new ConfirmedUserEmailComponent(player.Data.Email, player.Data.Subscribed),
                new UserSubscribeComponent());

            if (player.Data.Fraction is not null)
                user.AddComponent(new FractionGroupComponent(player.Data.Fraction));

            user.AddComponent(new UserGroupComponent(user));

            return user;
        }
    }
}
