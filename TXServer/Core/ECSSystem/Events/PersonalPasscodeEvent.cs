namespace TXServer.Core.ECSSystem.Events
{
	[SerialVersionUID(1439531278716)]
	public class PersonalPasscodeEvent : ECSEvent
	{
		[Protocol] public string Passcode { get; set; } = "j4xEgl7WRO9H7HwnK/R1c8FYws1jUdJorx2yoCB53Kw="; // hardcoded value!!!
	}
}
