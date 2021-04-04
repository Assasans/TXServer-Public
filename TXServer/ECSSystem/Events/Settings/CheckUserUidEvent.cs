using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437990639822L)]
	public class CheckUserUidEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			// TODO: check if uid is occupied
			bool emailIsOccupied = false;
			if (emailIsOccupied) 
			{
				player.SendEvent(new UserUidOccupiedEvent(Uid), entity);
				return;
			}

			player.SendEvent(new UserUidVacantEvent(Uid), entity);
		}

		public string Uid { get; set; }
	}
}
