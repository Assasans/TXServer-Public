#nullable enable

using System;

using TXServer.Core;

namespace TXServer.ECSSystem.Types.Punishments
{
	/// <summary>
	/// Represents an abstract punishment
	/// </summary>
	public abstract class Punishment
	{
		public PlayerData Player { get; protected set; }

		/// <summary>
		/// Punishment create time
		/// </summary>
		public DateTimeOffset CreateTime { get; protected set; }

		/// <summary>
		/// Ban reason, <see langword="null" /> if ban has not reason
		/// </summary>
		public string? Reason { get; set; }

		public Punishment(PlayerData player, DateTimeOffset createTime)
		{
			Player = player ?? throw new ArgumentNullException(nameof(player));

			CreateTime = createTime;
		}
	}
}
