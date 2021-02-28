using System.Numerics;

namespace TXServer.Core.ServerMapInformation
{
    public class SpawnPoint
    {
        public int Number { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
