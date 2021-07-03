using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1465192871085L)]
    public class ConfirmUserCountryEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Logger.Debug($"Got country code! '{CountryCode}'");
            player.Data.SetCountryCode(CountryCode);
        }
        public string CountryCode { get; set; }
    }
}
