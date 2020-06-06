using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1433752208915)]
    public class UserTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(string uid)
        {
            Entity entity = new Entity(new TemplateAccessor(new UserTemplate(), ""),
				new UserXCrystalsComponent(50000),
				new UserCountryComponent("RU"),
				new UserAvatarComponent("8b74e6a3-849d-4a8d-a20e-be3c142fd5e8"),
				new UserComponent(),
				new UserMoneyComponent(1000000),
				new TutorialCompleteIdsComponent(),
				new RegistrationDateComponent(),
				new LeagueGroupComponent(Leagues.GlobalItems.Silver),
				new UserStatisticsComponent(),
				new PersonalChatOwnerComponent(),
				new GameplayChestScoreComponent(),
				new UserRankComponent(101),
				new BlackListComponent(),
				new UserUidComponent(uid),
				new UserExperienceComponent(2000000),
				new QuestReadyComponent(),
				new UserPublisherComponent(),
				new FavoriteEquipmentStatisticsComponent(),
				new ConfirmedUserEmailComponent("none"),
				new UserSubscribeComponent(),
				new KillsEquipmentStatisticsComponent(),
				new BattleLeaveCounterComponent(),
				new UserReputationComponent(0.0));
			entity.Components.Add(new UserGroupComponent(entity));

			return entity;
		}
    }
}
