﻿using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Friend.Squad
{
    [SerialVersionUID(1507211574274L)]
    public class InviteToSquadEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach (long invitedUserId in InvitedUsersIds)
            {
                Player remotePlayer = Server.Instance.FindPlayerById(invitedUserId);

                remotePlayer.SendEvent(new InvitedToSquadEvent(player), remotePlayer.User);
            }
        }

        public long[] InvitedUsersIds { get; set; }
    }
}
