using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1437990639822L)]
	public class CheckUserUidEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			ICommand command = new SendEventCommand(new UserUidVacantEvent(Uid), entity);
			// TODO: check if uid is occupied
			bool emailIsOccupied = false;
			if (emailIsOccupied) 
			{
			    command = new SendEventCommand(new UserUidOccupiedEvent(Uid), entity);
			}

		    CommandManager.SendCommands(player, command);
		}

		public string Uid { get; set; }
	}
}
