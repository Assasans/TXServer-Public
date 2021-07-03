using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Railgun
{
    public class BaseRailgunChargingShotEvent : ECSEvent
    {
		public int ClientTime { get; set; }
    }
}
