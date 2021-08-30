using TXServer.Core;
using TXServer.Core.ChatCommands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Chat;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1512742576673L)]
	public class ElevatedAccessUserUnbanUserEvent : ECSEvent
	{
        public void Execute(Player player, Entity entity)
        {
            if (!player.Data.IsAdmin) return;

            ChatMessageReceivedEvent.SystemMessageTarget(ModCommands.Unban(player, new[] {Uid}), player);
        }

        public string Uid { get; set; }
    }
}
