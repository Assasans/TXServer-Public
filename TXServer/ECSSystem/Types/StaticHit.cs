using System.Numerics;

namespace TXServer.ECSSystem.Types
{
    public class StaticHit
	{
		public Vector3 Normal { get; set; }

		public Vector3 Position { get; set; }

		public override string ToString()
		{
			return string.Format("Position: {0}, Normal: {1}", Position, Normal);
		}
	}
}
