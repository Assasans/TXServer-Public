using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1504160222978L)]
	public class ChangeScreenEvent : ECSEvent
	{
		public ChangeScreenEvent(string currentScreen, string nextScreen, double duration)
		{
			CurrentScreen = currentScreen;
			NextScreen = nextScreen;
			Duration = duration;
		}

		public string CurrentScreen { get; set; }

		public string NextScreen { get; set; }

		public double Duration { get; set; }
	}
}
