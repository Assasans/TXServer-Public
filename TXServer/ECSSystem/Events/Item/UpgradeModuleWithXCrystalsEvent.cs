using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Item
{
    [SerialVersionUID(636407242256473252L)]
    public class UpgradeModuleWithXCrystalsEvent : UpgradeModuleWithCrystalsEvent
    {
        public new static void Execute(Player player, Entity marketItem) => TryUpgradeModule(player, marketItem, true);
    }
}
