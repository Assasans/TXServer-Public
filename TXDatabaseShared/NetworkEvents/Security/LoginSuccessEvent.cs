using Simple.Net;

namespace TXDatabase.NetworkEvents.Security {
    public struct LoginSuccessEvent : INetSerializable {
        public void Serialize(NetWriter writer) {}
        public void Deserialize(NetReader reader) {}
    }
}
