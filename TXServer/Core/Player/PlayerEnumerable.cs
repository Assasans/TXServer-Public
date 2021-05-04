using System.Collections.Generic;
using System.Linq;
using TXServer.ECSSystem.Base;

namespace TXServer.Core
{
    public static class PlayerEnumerable
    {
        public static void SharePlayers(this IEnumerable<IPlayerPart> players, params Player[] toShare) => SharePlayers(players, (IEnumerable<Player>)toShare);
        public static void SharePlayers(this IEnumerable<IPlayerPart> players, IEnumerable<Player> toShare) => SharePlayers(players.Select(x => x.Player), toShare);

        public static void SharePlayers(this IEnumerable<Player> players, params Player[] toShare) => SharePlayers(players, (IEnumerable<Player>)toShare);
        public static void SharePlayers(this IEnumerable<Player> players, IEnumerable<Player> toShare)
        {
            foreach (Player player in players)
                player.SharePlayers(toShare);
        }

        public static void UnsharePlayers(this IEnumerable<IPlayerPart> players, params Player[] toUnshare) => UnsharePlayers(players, (IEnumerable<Player>)toUnshare);
        public static void UnsharePlayers(this IEnumerable<IPlayerPart> players, IEnumerable<Player> toUnshare) => UnsharePlayers(players.Select(x => x.Player), toUnshare);

        public static void UnsharePlayers(this IEnumerable<Player> players, params Player[] toUnshare) => UnsharePlayers(players, (IEnumerable<Player>)toUnshare);
        public static void UnsharePlayers(this IEnumerable<Player> players, IEnumerable<Player> toUnshare)
        {
            foreach (Player player in players)
                player.UnsharePlayers(toUnshare);
        }

        public static void ShareEntities(this IEnumerable<IPlayerPart> players, params Entity[] entities) => ShareEntities(players, (IEnumerable<Entity>)entities);
        public static void ShareEntities(this IEnumerable<IPlayerPart> players, IEnumerable<Entity> entities) => ShareEntities(players.Select(x => x.Player), entities);

        public static void ShareEntities(this IEnumerable<Player> players, params Entity[] entities) => ShareEntities(players, (IEnumerable<Entity>)entities);
        public static void ShareEntities(this IEnumerable<Player> players, IEnumerable<Entity> entities)
        {
            foreach (Player player in players)
                player.ShareEntities(entities);
        }

        public static void UnshareEntities(this IEnumerable<IPlayerPart> players, params Entity[] entities) => UnshareEntities(players, (IEnumerable<Entity>)entities);
        public static void UnshareEntities(this IEnumerable<IPlayerPart> players, IEnumerable<Entity> entities) => UnshareEntities(players.Select(x => x.Player), entities);

        public static void UnshareEntities(this IEnumerable<Player> players, params Entity[] entities) => UnshareEntities(players, (IEnumerable<Entity>)entities);
        public static void UnshareEntities(this IEnumerable<Player> players, IEnumerable<Entity> entities)
        {
            foreach (Player player in players)
                player.UnshareEntities(entities);
        }

        public static void SendEvent(this IEnumerable<IPlayerPart> players, ECSEvent @event, params Entity[] entities) => SendEvent(players.Select(x => x.Player), @event, entities);
        public static void SendEvent(this IEnumerable<Player> players, ECSEvent @event, params Entity[] entities)
        {
            foreach (Player player in players)
                player.SendEvent(@event, entities);
        }
    }
}
