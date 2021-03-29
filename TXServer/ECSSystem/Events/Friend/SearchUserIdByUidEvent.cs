using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1469526368502L)]
    public class SearchUserIdByUidEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            bool isNotSelfUser = player.User.GetComponent<UserUidComponent>().Uid != Uid;

            // todo: search user in database
            Player searchedPlayer = Server.Instance.Connection.Pool.SingleOrDefault(controlledPlayer => controlledPlayer.User != null && controlledPlayer.User.GetComponent<UserUidComponent>().Uid == Uid && isNotSelfUser);
            long searchedPlayerId = 0;
            if (searchedPlayer != null) searchedPlayerId = searchedPlayer.User.EntityId;

            player.SendEvent(new SerachUserIdByUidResultEvent(searchedPlayer != null, searchedPlayerId), entity);
        }

        public string Uid { get; set; }
    }
}
