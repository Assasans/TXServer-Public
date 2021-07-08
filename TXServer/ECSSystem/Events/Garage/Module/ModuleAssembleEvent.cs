using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Garage.Module
{
    [SerialVersionUID(1485504324992L)]
    public class ModuleAssembleEvent : ECSEvent
    {
        public static void Execute(Player player, Entity marketItem, Entity user) =>
            player.Data.UpgradeModule(marketItem);
    }
}
