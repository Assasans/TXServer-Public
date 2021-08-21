using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Settings
{
    [SerialVersionUID(1465192871085L)]
    public class ConfirmUserCountryEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity) => player.Data.CountryCode = CountryCode;

        public string CountryCode { get; set; }
    }
}
