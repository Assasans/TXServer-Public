using TXServer.Core.Protocol;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.Events.Battle
{
    public struct MoveCommand
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