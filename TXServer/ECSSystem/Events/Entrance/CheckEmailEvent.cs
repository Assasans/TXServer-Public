using TXServer.Core;
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
				var addr = new System.Net.Mail.MailAddress(Email);

				// TODO: check if email is occupied
				bool emailIsOccupied = false;
				if (emailIsOccupied)
				{
					player.SendEvent(new EmailOccupiedEvent(Email), entity);
					return;
				}

				player.SendEvent(new EmailVacantEvent(Email), entity);
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
