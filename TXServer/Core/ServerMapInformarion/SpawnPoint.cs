using System.Numerics;

namespace TXServer.Core.ServerMapInformation
{
    public class SpawnPoint
    {
        public SpawnPoint()
        {
            Number = 50;
            Position = new Vector3(0,8,0);
            Rotation = new Quaternion();
        }

        public int Number { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
