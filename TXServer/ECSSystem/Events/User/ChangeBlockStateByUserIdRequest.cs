using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.User
{
    [SerialVersionUID(1507198221820L)]
    public class ChangeBlockStateByUserIdRequest : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            PlayerData remotePlayer = player.Server.Database.GetPlayerDataById(UserId);

            player.Data.ChangeBlockedPlayer(remotePlayer);
        }

        public long UserId { get; set; }
        public InteractionSource InteractionSource { get; set; }
        public long SourceId { get; set; }
    }
}
