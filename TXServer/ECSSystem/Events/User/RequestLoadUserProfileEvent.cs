using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1451368548585L)]
    public class RequestLoadUserProfileEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Player remotePlayer = Server.Instance.FindPlayerByUid(UserId);

            player.SharePlayers(remotePlayer);
            player.SendEvent(new UserProfileLoadedEvent(), remotePlayer.User);
        }

        public long UserId { get; set; }
    }
}
