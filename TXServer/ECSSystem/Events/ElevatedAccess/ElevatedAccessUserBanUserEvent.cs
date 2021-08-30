using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1503470104769L)]
    public class ElevatedAccessUserBanUserEvent : ElevatedAccessUserBasePunishEvent, ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (!player.Data.IsAdmin) return;

            ChatMessageReceivedEvent.SystemMessageTarget(
                Type.ToLower() == "warn"
                    ? ModCommands.Warn(player, new[] {Uid, Reason})
                    : ModCommands.Ban(player, new[] {Uid, Type, Reason}),
                player);
        }

        public string Type { get; set; }
    }
}
