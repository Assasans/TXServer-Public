using TXServer.Core.Protocol;
using TXServer.ECSSystem.Events.Battle.Weapon.Shot;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hammer
{
    [SerialVersionUID(-8245726943400840523L)]
	public class RemoteHammerShotEvent : RemoteShotEvent
	{
		public int RandomSeed { get; set; }
	}
}
