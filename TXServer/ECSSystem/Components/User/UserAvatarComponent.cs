using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.User
{
    [SerialVersionUID(1545809085571)]
    public class UserAvatarComponent : Component
    {
        public UserAvatarComponent(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}
