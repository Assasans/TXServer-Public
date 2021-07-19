using System.Collections.Generic;
using System.Linq;
using System.Net;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.User.Tutorial
{
    [SerialVersionUID(1505286737090)]
    public class TutorialCompleteIdsComponent : Component
    {
        public TutorialCompleteIdsComponent(List<ulong> completedIds, Player player)
        {
            // ReSharper disable once PossibleNullReferenceException
            CompletedIds = ((IPEndPoint) player.Connection.Socket.RemoteEndPoint).Address.Equals(IPAddress.Loopback)
                ? _allIds
                : _temporarilyBlockedIds.Concat(completedIds).ToList();
        }

        public List<ulong> CompletedIds { get; set; }
        public bool TutorialSkipped { get; set; }

        private readonly List<ulong> _allIds = new()
        {
            0x00000000190828D4,
            0x000000006C508645,
            0xFFFFFFFFFFF473B3,
            0x000000006C508646,
            0x000000006C508647,
            0x000000001DC04290,
            0x0000000066EFE2F8,
            0xFFFFFFFFD975A705,
            0xFFFFFFFFA0285472,
            0xFFFFFFFFF60C1145,
            0x000000006EC527F6
        };

        private readonly List<ulong> _temporarilyBlockedIds = new()
        {
            //0x00000000190828D4,
            0x000000006C508645,
            0xFFFFFFFFFFF473B3,
            0x000000006C508646,
            0x000000006C508647,
            0x000000001DC04290,
            0x0000000066EFE2F8,
            0xFFFFFFFFD975A705,
            0xFFFFFFFFA0285472,
            0xFFFFFFFFF60C1145,
            0x000000006EC527F6
        };
    }
}
