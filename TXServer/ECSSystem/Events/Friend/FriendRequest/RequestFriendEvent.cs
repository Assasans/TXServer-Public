﻿using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend
{
    [SerialVersionUID(1450168139800L)]
    public class RequestFriendEvent : FriendBaseEvent, ECSEvent
    {
        public void Execute(Player player, Entity clientSession)
        {
            Player remotePlayer = Server.Instance.FindPlayerById(User.EntityId);
            if (remotePlayer != null && remotePlayer.IsLoggedIn)
            {
                remotePlayer.Data.AddIncomingFriend(player.User.EntityId);
                remotePlayer.SendEvent(new IncomingFriendAddedEvent(player.User.EntityId), remotePlayer.User);
            }

            player.Data.AddOutgoingFriend(User.EntityId);
            player.UnshareEntity(remotePlayer.User);
            player.SendEvent(new OutgoingFriendAddedEvent(remotePlayer.User.EntityId), clientSession);
        }
    }
}
