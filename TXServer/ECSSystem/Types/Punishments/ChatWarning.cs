#nullable enable

using System;

using TXServer.Core;

namespace TXServer.ECSSystem.Types.Punishments
{
	/// <summary>
	/// Represents a chat warning
	/// </summary>
	public class ChatWarning : Punishment
	{
		public ChatWarning(PlayerData player, DateTimeOffset createTime) : base(player, createTime)
		{
		}
	}
}
