using Simple.Net;

namespace TXDatabase.NetworkEvents.Security {
    public struct LoginEvent : INetSerializable {
        public string encryptedAPIKey; // eg: D465-P634-UI41-T015
        public string encryptedAPIToken; // eg: 143-438-129-432-102-525

        public void Deserialize(NetReader reader) {
            encryptedAPIKey = reader.ReadString();
            encryptedAPIToken = reader.ReadString();
        }

        public void Serialize(NetWriter writer) {
            writer.Push(encryptedAPIKey);
            writer.Push(encryptedAPIToken);
        }
    }
}
