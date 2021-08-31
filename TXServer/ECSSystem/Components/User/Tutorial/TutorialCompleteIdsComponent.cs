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
        public TutorialCompleteIdsComponent(Player player)
        {
            SelfOnlyPlayer = player;
        }

        // ReSharper disable once PossibleNullReferenceException
        public IReadOnlyList<long> CompletedIds => ((IPEndPoint) SelfOnlyPlayer.Connection.Socket.RemoteEndPoint).Address.Equals(IPAddress.Loopback)
            ? _allIds
            : _temporarilyBlockedIds.Concat(SelfOnlyPlayer.Data.CompletedTutorials.ToIds()).ToList();

        public bool TutorialSkipped { get; set; }

        private readonly List<long> _allIds = new()
        {
            419965140,
            1817216581,
            -756813,
            1817216582,
            1817216583,
            499139216,
            1726997240,
            -646600955,
            -1607969678,
            -166981307,
            1858414582
        };

        private readonly List<long> _temporarilyBlockedIds = new()
        {
            // 419965140,
            1817216581,
            -756813,
            1817216582,
            1817216583,
            499139216,
            1726997240,
            -646600955,
            -1607969678,
            -166981307,
            1858414582
        };
    }
}
