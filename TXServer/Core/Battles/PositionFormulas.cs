using System;
using System.Numerics;

namespace TXServer.Core.Battles
{
    public class PositionFormulas
    {
        public static bool CheckOverflow(Vector3 vec)
        {
            const float maxValue = 655.36f;
            return Math.Abs(vec.X) > maxValue ||
                   Math.Abs(vec.Y) > maxValue ||
                   Math.Abs(vec.Z) > maxValue;
        }

        public static bool IsInsideBox(Vector3 point, Vector3 center, Vector3 size)
        {
            return point.X > center.X - size.X / 2 &&
                   point.Y > center.Y - size.Y / 2 &&
                   point.Z > center.Z - size.Z / 2 &&
                   point.X < center.X + size.X / 2 &&
                   point.Y < center.Y + size.Y / 2 &&
                   point.Z < center.Z + size.Z / 2;
        }
    }
}
