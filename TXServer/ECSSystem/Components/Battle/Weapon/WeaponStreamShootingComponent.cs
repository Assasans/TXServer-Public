using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(6803807621463709653L)]
	public class WeaponStreamShootingComponent : Component
	{
		[OptionalMapped]
		public TXDate StartShootingTime { get; set; }

		public int Time { get; set; }
	}
}