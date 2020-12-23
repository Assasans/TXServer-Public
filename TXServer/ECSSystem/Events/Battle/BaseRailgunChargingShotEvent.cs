using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    public class BaseRailgunChargingShotEvent : ECSEvent
    {
		public int ClientTime { get; set; }
    }
}