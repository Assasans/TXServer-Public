using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1522323975002L)]
	public class GetLeagueInfoEvent : ECSEvent
	{
        public void Execute(Player player, Entity entity)
            => player.SendEvent(new UpdateTopLeagueInfoEvent(UserId), entity);

        public long UserId { get; set; }
    }
}
