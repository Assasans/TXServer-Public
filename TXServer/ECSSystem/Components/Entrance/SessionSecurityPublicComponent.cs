using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1439792100478)]
    public sealed class SessionSecurityPublicComponent : Component
    {
        public SessionSecurityPublicComponent(string publicKey)
            => PublicKey = publicKey;

        public string PublicKey { get; set; }
    }
}
