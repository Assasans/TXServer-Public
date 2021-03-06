using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1439792100478)]
    public sealed class SessionSecurityPublicComponent : Component
    {
        public SessionSecurityPublicComponent(string publicKey)
            => PublicKey = publicKey;

        public string PublicKey { get; set; } = "AI5q8XLJibe9vwx50OoS4A6nHai3oNd6U3ct96535B3azEoHfWKXQYOV6CbJfXUOBAoUvDzVbJGiOXPED9k0jAM=:AQAB"; // hardcoded value!!!
    }
}
