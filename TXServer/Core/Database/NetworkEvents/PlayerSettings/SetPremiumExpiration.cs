using Simple.Net;

namespace TXServer.Core.Database.NetworkEvents.PlayerSettings
{
    public struct SetPremiumExpiration : INetSerializable
    {
        public long uid;
        public long expiration;

        public void Deserialize(NetReader reader)
        {
            uid = reader.ReadInt64();
            expiration = reader.ReadInt64();
        }

        public void Serialize(NetWriter writer)
        {
            writer.Push(uid);
            writer.Push(expiration);
        }
    }
}
