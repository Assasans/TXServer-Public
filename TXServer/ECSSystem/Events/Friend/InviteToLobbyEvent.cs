using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1497002374017L)]
    public class InviteToLobbyEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach(long userId in InvitedUserIds)
            {
                Player remotePlayer = Server.Instance.FindPlayerById(userId);

                if (remotePlayer != null)
                {
                    if (!remotePlayer.EntityList.Contains(player.User))
                        remotePlayer.ShareEntity(player.User);
                    remotePlayer.ShareEntity(InviteFriendToBattleNotificationTemplate.CreateEntity(player, entity));
                    remotePlayer.SendEvent(new ShowNotificationGroupEvent(1), entity);
                }
            }
        }

        public long[] InvitedUserIds { get; set; }
    }
}
