using System.Numerics;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(8445798616771064825L)]
	public class ShaftAimingTargetPointComponent : Component
	{
		public bool IsInsideTankPart { get; set; }

		[OptionalMapped]
		public Vector3 Point { get; set; }
	}
}