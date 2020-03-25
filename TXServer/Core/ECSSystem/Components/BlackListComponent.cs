using System.Collections.Generic;

namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1507197930106L)]
    public class BlackListComponent : Component
    {
        public List<long> BlockedUsers { get; set; } = new List<long>();
    }
}
