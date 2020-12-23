using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(-8245726943400840523L)]
	public class RemoteHammerShotEvent : RemoteShotEvent
	{
		public int RandomSeed { get; set; }
	}
}