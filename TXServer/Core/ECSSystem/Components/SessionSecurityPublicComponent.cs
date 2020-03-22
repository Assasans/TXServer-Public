namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1439792100478)]
    public sealed class SessionSecurityPublicComponent : Component
    {
        [Protocol] public string PublicKey { get; set; } = "AI5q8XLJibe9vwx50OoS4A6nHai3oNd6U3ct96535B3azEoHfWKXQYOV6CbJfXUOBAoUvDzVbJGiOXPED9k0jAM=:AQAB"; // hardcoded value!!!
    }
}