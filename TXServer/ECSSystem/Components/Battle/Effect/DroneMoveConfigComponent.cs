using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Effect {
    [SerialVersionUID(3441234123559L)]
    public class DroneMoveConfigComponent : Component
    {
        public DroneMoveConfigComponent(MatchPlayer matchPlayer)
        {
            Acceleration = 30;
            SpawnPosition = matchPlayer.TankPosition;
            FlyPosition = new Vector3(0, 4, 0);
            RotationSpeed = 10;
            MoveSpeed = 120;
        }

        public float Acceleration { get; set; }
        public Vector3 SpawnPosition { get; set; }
        public Vector3 FlyPosition { get; set; }
        public float RotationSpeed { get; set; }
        public float MoveSpeed { get; set; }
    }
}
