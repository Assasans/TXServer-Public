using System;
using System.Numerics;
using System.Threading;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Time;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1461735527769)]
    public class InitializeTimeCheckerEvent : ECSEvent
    {
        public void Execute(Player player, Entity incarnation)
        {
            Entity tank = player.ReferencedEntities["tank"];
            
            CommandManager.SendCommands(player,
                new ComponentAddCommand(tank,
                    new TankMovementComponent(
                        new Movement(new Vector3(20, 18, 98), Vector3.Zero, Vector3.Zero, new Quaternion(0, 0, 0, 0)),
                        new MoveControl(0, 0), 0, 0))
                // new SendEventCommand(new HealthChangedEvent()),
                // new ComponentRemoveCommand(player.ReferencedEntities["round"], typeof(RoundWarmingUpStateComponent))
                );
            
            new Thread(() =>
            {
                Thread.Sleep(12000);
                CommandManager.SendCommands(player,
                    new SendEventCommand(new HealthChangedEvent()));
            }).Start();
            
            // CommandManager.SendCommands(player, 
            //     new EntityShareCommand(new Entity(
            //         new BattleTimeIndicatorComponent("Hi", 10F),
            //         tank.GetComponent<BattleGroupComponent>()
            //     )));
            //todo ?
        }
    }
}