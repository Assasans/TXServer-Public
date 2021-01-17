using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1547709389410L)]
	public class EnterBattleLobbyFailedEvent : ECSEvent
	{
		public EnterBattleLobbyFailedEvent(bool alreadyInLobby, bool lobbyIsFull)
		{
			AlreadyInLobby = alreadyInLobby;
			LobbyIsFull = lobbyIsFull;
		}
		public bool AlreadyInLobby { get; set; }
		public bool LobbyIsFull { get; set; }
	}
}