using System.Numerics;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(1399558738794728790)]
    public class UserReadyToBattleComponent : Component
    {
		public void OnAttached(Player player, Entity battleUser)
		{
			CommandManager.SendCommands(player,
				new ComponentAddCommand(player.ReferencedEntities["tank"], new TankMovementComponent(new Movement(new Vector3(0, 100, 0), Vector3.Zero, Vector3.Zero, Quaternion.Identity), new MoveControl(), 0, 0)),
				new ComponentRemoveCommand(player.ReferencedEntities["tank"], typeof(TankSpawnStateComponent)),
				new ComponentAddCommand(player.ReferencedEntities["tank"], new TankVisibleStateComponent()),
				new ComponentAddCommand(player.ReferencedEntities["tank"], new TankSemiActiveStateComponent(1)),
				new ComponentAddCommand(player.ReferencedEntities["tank"], new TankMovableComponent()),
				new ComponentAddCommand(player.ReferencedEntities["tank"], new TankActiveStateComponent()));
		}
	}
}