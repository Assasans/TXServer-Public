using Simple.Net;

namespace TXServer.Core.Database.NetworkEvents.PlayerAuth
{
    public struct GetUserViaUsername : INetSerializable
    {
        public int packetId;
        public string encryptedUsername;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(encryptedUsername);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            encryptedUsername = reader.ReadString();
        }
    }
}
