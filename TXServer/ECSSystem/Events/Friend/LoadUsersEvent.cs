using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1458555246853L)]
    public class LoadUsersEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            List<ICommand> commands = new List<ICommand>();

            foreach (long id in UsersId)
            {
                Entity found;
                try
                {
                    found = Server.Instance.Connection.Pool
                        .Where(p => p.User != null && p.User.EntityId != player.User.EntityId && id == p.User.EntityId)
                        .Select(p => p.User)
                        .Single();
                }
                catch (InvalidOperationException)
                {
                    found = new Entity(id, new TemplateAccessor(new UserTemplate(), ""),
                        new UserXCrystalsComponent(50000),
                        new UserCountryComponent("RU"),
                        new UserAvatarComponent("8b74e6a3-849d-4a8d-a20e-be3c142fd5e8"),
                        new UserComponent(),
                        new UserMoneyComponent(1000000),
                        new FractionGroupComponent(Fractions.GlobalItems.Frontier),
                        new UserDailyBonusCycleComponent(1),
                        new TutorialCompleteIdsComponent(),
                        new RegistrationDateComponent(),
                        new LeagueGroupComponent(Leagues.GlobalItems.Silver),
                        new UserStatisticsComponent(),
                        new PersonalChatOwnerComponent(),
                        new GameplayChestScoreComponent(),
                        new UserRankComponent(101),
                        new BlackListComponent(),
                        new UserUidComponent("null"),
                        new FractionUserScoreComponent(500),
                        new UserExperienceComponent(2000000),
                        new QuestReadyComponent(),
                        new UserPublisherComponent(),
                        new FavoriteEquipmentStatisticsComponent(),
                        new UserDailyBonusReceivedRewardsComponent(),
                        new ConfirmedUserEmailComponent("none"),
                        new UserSubscribeComponent(),
                        new KillsEquipmentStatisticsComponent(),
                        new BattleLeaveCounterComponent(0, 0),
                        new UserReputationComponent(0.0),
                        new UserGroupComponent(id));
                }

                commands.Add(new EntityShareCommand(found));
            }

            commands.Add(new SendEventCommand(new UsersLoadedEvent(RequestEntityId), entity));
            CommandManager.SendCommands(player, commands.ToArray());
        }

        public long RequestEntityId { get; set; }

        public HashSet<long> UsersId { get; set; }
    }
}
