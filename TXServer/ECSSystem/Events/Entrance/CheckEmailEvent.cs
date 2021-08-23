using System.Net.Mail;
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
                MailAddress address = new MailAddress(Email);
                // TODO(Assasans): Handle IncludeUnconfirmed property
                if (player.Server.Database.IsEmailAvailable(address.Address))
                    player.SendEvent(new EmailVacantEvent(Email), entity);
                else
                    player.SendEvent(new EmailOccupiedEvent(Email), entity);
            }
            catch
            {
                player.SendEvent(new EmailInvalidEvent(Email), entity);
            }
        }

        public string Email { get; set; }
        public bool IncludeUnconfirmed { get; set; }
    }
}
