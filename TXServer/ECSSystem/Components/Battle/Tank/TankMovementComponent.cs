using System.Numerics;
using System.Runtime.InteropServices;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Tank
{
    [SerialVersionUID(-615965945505672897)]
    public class TankMovementComponent : Component
    {
        public TankMovementComponent(Movement movement, MoveControl moveControl, float weaponRotation, float weaponControl)
        {
            Movement = movement;
            MoveControl = moveControl;
            WeaponRotation = weaponRotation;
            WeaponControl = weaponControl;
        }

        public Movement Movement { get; set; }

        public MoveControl MoveControl { get; set; }

        public float WeaponRotation { get; set; }

        public float WeaponControl { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct Movement
    {
        public Movement(Vector3 position, Vector3 velocity, Vector3 angularVelocity, Quaternion orientation)
        {
            Position = position;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
            Orientation = orientation;
        }

        public Vector3 Position { get; set; }

        public Vector3 Velocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

        public Quaternion Orientation { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MoveControl
    {
        public MoveControl(float moveAxis, float turnAxis)
        {
            MoveAxis = moveAxis;
            TurnAxis = turnAxis;
        }

        public float MoveAxis { get; set; }

        public float TurnAxis { get; set; }
    }
}
