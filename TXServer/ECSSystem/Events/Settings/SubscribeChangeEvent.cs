using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1482844606270L)]
	public class SubscribeChangeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			player.Data.SetSubscribed(Subscribed);
			
			CommandManager.SendCommands(player, new ComponentChangeCommand(entity, new UserSubscribeComponent(Subscribed)));
		}
		public bool Subscribed { get; set; }
	}
}
