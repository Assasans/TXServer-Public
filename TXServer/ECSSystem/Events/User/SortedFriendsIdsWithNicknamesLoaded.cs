using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1498741007777L)]
    public class SortedFriendsIdsWithNicknamesLoaded : ECSEvent
    {
        public SortedFriendsIdsWithNicknamesLoaded(Dictionary<long, string> friendsIdsAndNicknames)
        {
            FriendsIdsAndNicknames = friendsIdsAndNicknames;
        }

        public Dictionary<long, string> FriendsIdsAndNicknames { get; set; }
    }
}
