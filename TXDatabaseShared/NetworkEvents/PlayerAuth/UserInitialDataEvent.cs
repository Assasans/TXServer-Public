using Simple.Net;

namespace TXDatabase.NetworkEvents.PlayerAuth
{
    public struct UserInitialDataEvent : INetSerializable
    {
        public int packetId;
        public long uid;
        public string username;
        public string hashedPassword;
        public string email;
        public bool emailVerified;
        public string hardwareId;
        public string hardwareToken;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(uid);
            if (uid == -1) return;
            writer.Push(username);
            writer.Push(hashedPassword);
            writer.Push(email);
            writer.Push(emailVerified);
            writer.Push(hardwareId);
            writer.Push(hardwareToken);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            uid = reader.ReadInt64();
            if (uid == -1) return;
            username = reader.ReadString();
            hashedPassword = reader.ReadString();
            email = reader.ReadString();
            emailVerified = reader.ReadBool();
            hardwareId = reader.ReadString();
            hardwareToken = reader.ReadString();
        }
    }
}
