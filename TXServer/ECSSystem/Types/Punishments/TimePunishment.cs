#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;

using TXServer.Core;

namespace TXServer.ECSSystem.Types.Punishments
{
	/// <summary>
	/// Represents an abstract pushishment with duration
	/// </summary>
	public abstract class TimePunishment : Punishment
	{
		/// <summary>
		/// Ban duration, <see langword="null" /> if ban is permanent
		/// </summary>
		public TimeSpan? Duration { get; set; }

		// TODO(Assasans): Handle negative TimeSpan
		public TimeSpan? TimeLeft => ExpireTime - DateTimeOffset.Now;

		/// <summary>
		/// Ban expiration time, <see langword="null" /> if ban is permanent
		/// </summary>
		public DateTimeOffset? ExpireTime => Duration != null ? CreateTime + Duration : null;

		public bool IsPermanent => Duration == null;

		public bool IsExpired => DateTimeOffset.Now >= ExpireTime;

		public TimePunishment(PlayerData player, DateTimeOffset createTime) : base(player, createTime)
		{
		}
	}
}
