using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1507197930106L)]
    public class BlackListComponent : Component
    {
        public List<long> BlockedUsers { get; set; } = new List<long>();
    }
}
