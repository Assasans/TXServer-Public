using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(1436520497855L)]
    public class UserCountComponent : Component
    {
        public UserCountComponent(int userCount)
        {
            UserCount = userCount;
        }

        public int UserCount { get; set; }
    }
}