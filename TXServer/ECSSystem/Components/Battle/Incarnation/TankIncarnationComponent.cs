using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle;

namespace TXServer.ECSSystem.Components.Battle.Incarnation
{
    [SerialVersionUID(1478091189336)]
    public class TankIncarnationComponent : Component
    {
        public void OnAttached(Player player, Entity tank)
        {
            CommandManager.SendCommands(player, new EntityShareCommand(TankIncarnationTemplate.CreateEntity(tank)));
        }
    }
}