using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Entrance;

namespace TXServer.ECSSystem.Events.Entrance.RestorePassword
{
	[SerialVersionUID(1460106433434L)]
	public class RestorePasswordByEmailEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			entity.AddComponent(new RestorePasswordCodeSentComponent(Email));
		}
		public string Email { get; set; }
	}
}
