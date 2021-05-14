using System.Numerics;

namespace TXServer.Core.ServerMapInformation
{
    public class TeleportPoint
    {
        public TeleportPoint(string name, Vector3 position, Quaternion rotation)
        {
            Name = name;
            Position = position;
            Rotation = rotation;
        }

        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
