using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1454585264587L)]
    public class InviteFriendToBattleNotificationTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Player player, Entity entity)
        {
            Entity notification = new Entity(new TemplateAccessor(new InviteFriendToBattleNotificationTemplate(), "notification/invitefriendtobattle"),
                new NotificationGroupComponent(entity),
                new NotificationComponent(NotificationPriority.MESSAGE),
                player.User.GetComponent<UserGroupComponent>(),
                player.BattlePlayer.Battle.BattleEntity.GetComponent<BattleModeComponent>(),
                player.BattlePlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>(),
                player.BattlePlayer.Battle.BattleEntity.GetComponent<MapGroupComponent>());
            player.User.AddComponent(new BattleSelectComponent());
            return notification;
        }
    }
}
