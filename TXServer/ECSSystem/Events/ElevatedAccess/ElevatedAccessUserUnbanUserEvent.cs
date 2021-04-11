using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.ElevatedAccess
{
    [SerialVersionUID(1512742576673L)]
	public class ElevatedAccessUserUnbanUserEvent : ECSEvent
	{
		public string Uid { get; set; }
	}
}