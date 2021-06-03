using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Events.Battle.Movement
{
    public struct MoveCommand
    {
        [OptionalMapped]
        public Components.Battle.Tank.Movement? Movement { get; set; }

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

        public bool IsDiscrete() => IsFloatDiscrete(TankControlVertical) &&
                                    IsFloatDiscrete(TankControlHorizontal) && IsFloatDiscrete(WeaponRotationControl);

        private static bool IsFloatDiscrete(float val) => val == 0.0 || val == 1.0 || val == -1.0;
    }
}
