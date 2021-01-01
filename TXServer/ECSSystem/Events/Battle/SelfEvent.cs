using System.Reflection;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    public static class SelfEvent
    {
		public static T ToRemoteEvent<T>(this ISelfEvent selfEvent) where T : IRemoteEvent, new()
		{
			T remoteEvent = new T();
			foreach (PropertyInfo info in selfEvent.GetType().GetProperties())
			{
				typeof(T).GetProperty(info.Name, info.PropertyType).SetValue(remoteEvent, info.GetValue(selfEvent));
			}
			return remoteEvent;
		}

		public static void Execute(this ISelfEvent selfEvent, Player player, Entity tankPart)
        {
			player.BattleLobbyPlayer.BattlePlayer.TranslatedEvents[selfEvent.GetType()] = new TranslatedEvent(selfEvent.ToRemoteEvent(), tankPart);
        }
	}
}