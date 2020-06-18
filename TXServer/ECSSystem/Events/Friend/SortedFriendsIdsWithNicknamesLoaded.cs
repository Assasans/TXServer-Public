using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1498741007777L)]
    public class SortedFriendsIdsWithNicknamesLoaded : ECSEvent
    {
        public byte /*Dictionary<long, string>*/ FriendsIdsAndNicknames { get; set; } = 0;
    }
}
