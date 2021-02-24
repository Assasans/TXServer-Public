using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.Core.RemoteDatabase;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1482844606270L)]
	public class SubscribeChangeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			if (RemoteDatabase.isInitilized)
				RemoteDatabase.Users.SetIsSubscribed(player.tempRow.username, Subscribed);
			player.tempRow.isSubscribed = Subscribed;
			ICommand command;
			if (Subscribed)
            {
				command = new ComponentAddCommand(player.User, new UserSubscribeComponent());
			} 
			else
            {
				command = new ComponentRemoveCommand(player.User, typeof(UserSubscribeComponent));
            }
			CommandManager.SendCommands(player, command);
		}
		public bool Subscribed { get; set; }
	}
}
