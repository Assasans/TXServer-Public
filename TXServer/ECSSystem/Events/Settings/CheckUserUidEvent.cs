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
			// TODO: check if uid is occupied in db
            if (Server.Instance.FindPlayerByUid(Uid) != null)
			{
				player.SendEvent(new UserUidOccupiedEvent(Uid), entity);
				return;
			}

			player.SendEvent(new UserUidVacantEvent(Uid), entity);
		}

		public string Uid { get; set; }
	}
}
