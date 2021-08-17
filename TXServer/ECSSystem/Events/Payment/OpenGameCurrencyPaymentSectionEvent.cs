using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Payment
{
    [SerialVersionUID(1455283639698L)]
	public class OpenGameCurrencyPaymentSectionEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity) => player.UpdateShop();
    }
}
