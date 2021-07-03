using System.Net.Mail;
using TXServer.Core;
using TXServer.Core.Database;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(635906273125139964L)]
	public class CheckEmailEvent : ECSEvent
	{
		public CheckEmailEvent(string email)
		{
			Email = email;
		}

		public void Execute(Player player, Entity entity)
		{
			try
			{
				var addr = new MailAddress(Email);

                if (Server.DatabaseNetwork.IsReady)
                    PacketSorter.EmailAvailable(addr.Address, result =>
                    {
                        if (result.result)
                            player.SendEvent(new EmailVacantEvent(Email), entity);
                        else player.SendEvent(new EmailOccupiedEvent(Email), entity);
                    });
                else
                {
                    /*bool emailIsOccupied = false;
                    if (emailIsOccupied)
                    {
                        player.SendEvent(new EmailOccupiedEvent(Email), entity);
                        return;
                    }*/

                    player.SendEvent(new EmailVacantEvent(Email), entity);
                }
			}
			catch
			{
				player.SendEvent(new EmailInvalidEvent(Email), entity);
			}
		}

		public string Email { get; set; } = "";
		public bool IncludeUnconfirmed { get; set; }
	}
}
