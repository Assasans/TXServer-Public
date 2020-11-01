using System.Numerics;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Types
{
    public class HitTarget
	{
		public Entity Entity { get; set; }

		public Vector3 HitDirection { get; set; }

		public float HitDistance { get; set; }

		public Entity IncarnationEntity { get; set; }

		public Vector3 LocalHitPoint { get; set; }

		public Vector3 TargetPosition { get; set; }
	}
}
