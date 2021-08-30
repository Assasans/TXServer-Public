using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.User
{
    [SerialVersionUID(1506939739582L)]
    public class ReportUserByUserId : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            // todo: report system / add user to a check list
            PlayerData data = player.Server.Database.GetPlayerDataById(UserId);

            player.Data.AddReportedPlayer(data);
        }

        public long UserId { get; set; }
        public InteractionSource InteractionSource { get; set; }
        public long SourceId { get; set; }
    }

}
