using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.Database;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1439375251389)]
    public class IntroduceUserByUidEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (!player.CheckInviteCode(Uid))
            {
                player.SendEvent(new UidInvalidEvent(), entity);
                player.SendEvent(new LoginFailedEvent(), entity);
                return;
            }

            PlayerData data = player.Server.Database.GetPlayerData(Uid);
            if (data == null)
            {
                player.SendEvent(new UidInvalidEvent(), entity);
                player.SendEvent(new LoginFailedEvent(), entity);
                return;
            }

            data.Player = player;
            player.Data = data;

            player.SendEvent(new PersonalPasscodeEvent(), entity);
        }

        public string Uid { get; set; }
    }
}
