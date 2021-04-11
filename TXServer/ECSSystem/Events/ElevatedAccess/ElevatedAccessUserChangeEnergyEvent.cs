using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
	[SerialVersionUID(1511430732717L)]
	public class ElevatedAccessUserChangeEnergyEvent : ECSEvent
	{
		// not working anymore, collectable energy was removed
		public int Count { get; set; }
	}
}