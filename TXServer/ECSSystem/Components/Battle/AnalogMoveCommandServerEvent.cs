using System.Numerics;
using System.Runtime.InteropServices;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(2596682299194665575)]
    public class AnalogMoveCommandServerEvent : ECSEvent
    {
        public AnalogMoveCommandServerEvent(Movement movement, MoveControl moveControl, float weaponRotation, float weaponControl)
        {
            Movement = movement;
            MoveControl = moveControl;
            WeaponRotation = weaponRotation;
            WeaponControl = weaponControl;
        }

        [OptionalMapped]
        public Movement Movement { get; set; }
        
        [OptionalMapped]
        public MoveControl MoveControl { get; set; }
        
        [OptionalMapped]
        public float WeaponRotation { get; set; }
        
        [OptionalMapped]
        public float WeaponControl { get; set; }
    }
}