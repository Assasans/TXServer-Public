using Simple.Net;

namespace TXDatabase.NetworkEvents.PlayerAuth
{
    public struct RegsiterUserRequest : INetSerializable
    {
        public int packetId;
        public string encryptedUsername;
        public string encryptedHashedPassword;
        public string encryptedEmail;
        public string encryptedHardwareId;
        public string encryptedHardwareToken;
        public string encryptedCountryCode;
        public bool subscribed;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(encryptedUsername);
            writer.Push(encryptedHashedPassword);
            writer.Push(encryptedEmail);
            writer.Push(encryptedHardwareId);
            writer.Push(encryptedHardwareToken);
            writer.Push(encryptedCountryCode);
            writer.Push(subscribed);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            encryptedUsername = reader.ReadString();
            encryptedHashedPassword = reader.ReadString();
            encryptedEmail = reader.ReadString();
            encryptedHardwareId = reader.ReadString();
            encryptedHardwareToken = reader.ReadString();
            encryptedCountryCode = reader.ReadString();
            subscribed = reader.ReadBool();
        }
    }
}
