using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Entrance
{
	[SerialVersionUID(1439808320725L)]
	public class InviteComponent : Component
	{
        public InviteComponent(bool showScreenOnEntrance, string inviteCode)
        {
			ShowScreenOnEntrance = showScreenOnEntrance;
			InviteCode = inviteCode;
        }

        public bool ShowScreenOnEntrance { get; set; }
		[OptionalMapped]
		public string InviteCode { get; set; }
	}
}
