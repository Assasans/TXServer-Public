using System.Collections.Generic;
using TXServer.ECSSystem.Base;

namespace TXServer.Core
{
    public static class PlayerPart
    {
        public static void SharePlayers(this IPlayerPart player, params Player[] toShare) => SharePlayers(player, (IEnumerable<Player>)toShare);
        public static void SharePlayers(this IPlayerPart player, IEnumerable<Player> toShare) => player.Player.SharePlayers(toShare);

        public static void UnsharePlayers(this IPlayerPart player, params Player[] toUnshare) => UnsharePlayers(player, (IEnumerable<Player>)toUnshare);
        public static void UnsharePlayers(this IPlayerPart player, IEnumerable<Player> toUnshare) => player.Player.UnsharePlayers(toUnshare);

        public static void ShareEntities(this IPlayerPart player, params Entity[] entities) => ShareEntities(player, (IEnumerable<Entity>)entities);
        public static void ShareEntities(this IPlayerPart player, IEnumerable<Entity> entities) => player.Player.ShareEntities(entities);

        public static void UnshareEntities(this IPlayerPart player, params Entity[] entities) => UnshareEntities(player, (IEnumerable<Entity>)entities);
        public static void UnshareEntities(this IPlayerPart player, IEnumerable<Entity> entities) => player.Player.UnshareEntities(entities);

        public static void SendEvent(this IPlayerPart player, ECSEvent @event, params Entity[] entities) => player.Player.SendEvent(@event, entities);
    }
}
