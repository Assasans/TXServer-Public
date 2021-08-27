using System.Numerics;

namespace TXServer.Core.ServerMapInformation
{
    public class Bonus
    {
        public Bonus(bool hasParachute, Vector3 position)
        {
            HasParachute = hasParachute;
            Position = position;
        }

        public int Number { get; set; }
        public bool HasParachute { get; set; }
        public Vector3 Position { get; set; }
    }
}
