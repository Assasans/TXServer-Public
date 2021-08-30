using System.Linq;
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
            // TODO(Assasans): Database only saves PlayerData, but UserProfileLoadedEvent requires Player entity to be shared
            Player remotePlayer = Server.Instance.Connection.Pool.SingleOrDefault(player => player.Data.UniqueId == UserId);

            player.SharePlayers(remotePlayer);
            player.SendEvent(new UserProfileLoadedEvent(), remotePlayer.User);
        }

        public long UserId { get; set; }
    }
}
