using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(6959116100408127452)]
    public class MoveCommandEvent : ECSEvent
    {
        public MoveCommandEvent(MoveCommand moveCommand) => MoveCommand = moveCommand;

        public MoveCommand MoveCommand { get; set; }

        public void Execute(Player player, Entity entity)
        {
            Console.WriteLine(MoveCommand.ToString());
        }
    }

    public class MoveCommand
    {
        [OptionalMapped]
        public Movement? Movement { get; set; }

        [OptionalMapped]
        public float? WeaponRotation { get; set; }

        [OptionalMapped]
        public float TankControlVertical { get; set; }

        [OptionalMapped]
        public float TankControlHorizontal { get; set; }

        [OptionalMapped]
        public float WeaponRotationControl { get; set; }

        public int ClientTime { get; set; }

        public override string ToString() =>
            $"MoveCommand[Movement={Movement}, WeaponRotation={WeaponRotation}]";

        public bool IsDiscrete() => this.IsFloatDiscrete(TankControlVertical) &&
                                    IsFloatDiscrete(TankControlHorizontal) && IsFloatDiscrete(WeaponRotationControl);

        private bool IsFloatDiscrete(float val) => val == 0.0 || val == 1.0 || val == -1.0;
    }
}