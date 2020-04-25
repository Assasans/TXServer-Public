using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(7453043498913563889)]
    public sealed class UserGroupComponent : GroupComponent
    {
        public UserGroupComponent(Entity Key) : base(Key)
        {
        }

        public UserGroupComponent(long Key) : base(Key)
        {
        }
    }
}
