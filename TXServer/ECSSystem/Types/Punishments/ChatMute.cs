#nullable enable

using System;

using TXServer.Core;

namespace TXServer.ECSSystem.Types.Punishments
{
	/// <summary>
	/// Represents a chat mute
	/// </summary>
	public class ChatMute : TimePunishment
	{
		public ChatMute(PlayerData player, DateTimeOffset createTime) : base(player, createTime)
		{
		}
	}
}
