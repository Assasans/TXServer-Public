using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1548677305789L)]
    public class OpenCustomLobbyPriceComponent : Component
    {
        public OpenCustomLobbyPriceComponent(long openPrice)
        {
            this.OpenPrice = openPrice;
        }

        public long OpenPrice { get; set; }
    }
}