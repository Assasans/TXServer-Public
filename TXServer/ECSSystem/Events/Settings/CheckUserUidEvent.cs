using TXServer.Core;
using TXServer.Core.Database;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437990639822L)]
	public class CheckUserUidEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            if (Server.DatabaseNetwork.IsReady)
                PacketSorter.UsernameAvailable(Uid, response =>
                {
                    if (response.result)
                        player.SendEvent(new UserUidVacantEvent(Uid), entity);
                    else player.SendEvent(new UserUidOccupiedEvent(Uid), entity);
                });
            else
            {
                if (Server.Instance.FindPlayerByUsername(Uid) != null)
                    player.SendEvent(new UserUidOccupiedEvent(Uid), entity);
                else player.SendEvent(new UserUidVacantEvent(Uid), entity);
            }
		}

		public string Uid { get; set; }
	}
}
